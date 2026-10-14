using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Data;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Services;

public class OnayService : IOnayService
{
    private const string GuncelOlmayanKayitMesaji =
        "Bu ceza siz sayfayı açtıktan sonra başka bir kullanıcı tarafından değiştirildi. Lütfen sayfayı yenileyip tekrar deneyin.";

    private const int AciklamaMaksimumUzunluk = 500;

    private readonly ApplicationDbContext _context;

    public OnayService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sonuc> OnayaGonderAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici)
    {
        var ceza = await CezaGetirAsync(cezaId, rowVersion);
        if (ceza is null)
            return Sonuc.Hatali("Ceza kaydı bulunamadı.");

        if (!ceza.RowVersion.SequenceEqual(rowVersion))
            return Sonuc.Hatali(GuncelOlmayanKayitMesaji);

        if (!OnayAkisi.OnayaGonderebilirMi(ceza, kullanici))
            return Sonuc.Hatali("Bu cezayı onaya gönderme yetkiniz bulunmuyor.");

        var aciklama = ceza.Durum == CezaDurumu.Reddedildi
            ? "Düzeltilerek yeniden onaya gönderildi."
            : "Onaya gönderildi.";

        return await DurumDegistirAsync(ceza, CezaDurumu.YoneticiOnayinda,
            IslemTipi.OnayaGonderildi, kullanici, aciklama);
    }

    public async Task<Sonuc> OnaylaAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? aciklama)
    {
        if (aciklama?.Length > AciklamaMaksimumUzunluk)
            return Sonuc.Hatali($"Açıklama en fazla {AciklamaMaksimumUzunluk} karakter olabilir.");

        var ceza = await CezaGetirAsync(cezaId, rowVersion);
        if (ceza is null)
            return Sonuc.Hatali("Ceza kaydı bulunamadı.");

        if (!ceza.RowVersion.SequenceEqual(rowVersion))
            return Sonuc.Hatali(GuncelOlmayanKayitMesaji);

        if (!OnayAkisi.KararVerebilirMi(ceza, kullanici))
            return Sonuc.Hatali("Bu aşamada onay yetkiniz bulunmuyor.");

        var yeniDurum = OnayAkisi.OnaySonrasiDurum(ceza.Durum);

        return await DurumDegistirAsync(ceza, yeniDurum, IslemTipi.Onaylandi,
            kullanici, string.IsNullOrWhiteSpace(aciklama) ? null : aciklama.Trim());
    }

    public async Task<Sonuc> ReddetAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? retNedeni)
    {
        if (string.IsNullOrWhiteSpace(retNedeni))
            return Sonuc.Hatali("Ret nedeni zorunludur.");

        if (retNedeni.Length > AciklamaMaksimumUzunluk)
            return Sonuc.Hatali($"Ret nedeni en fazla {AciklamaMaksimumUzunluk} karakter olabilir.");

        var ceza = await CezaGetirAsync(cezaId, rowVersion);
        if (ceza is null)
            return Sonuc.Hatali("Ceza kaydı bulunamadı.");

        if (!ceza.RowVersion.SequenceEqual(rowVersion))
            return Sonuc.Hatali(GuncelOlmayanKayitMesaji);

        if (!OnayAkisi.KararVerebilirMi(ceza, kullanici))
            return Sonuc.Hatali("Bu aşamada ret yetkiniz bulunmuyor.");

        return await DurumDegistirAsync(ceza, CezaDurumu.Reddedildi, IslemTipi.Reddedildi,
            kullanici, retNedeni.Trim());
    }

    private async Task<TrafikCezasi?> CezaGetirAsync(int cezaId, byte[] rowVersion)
    {
        var ceza = await _context.TrafikCezalari.FirstOrDefaultAsync(c => c.Id == cezaId);

        if (ceza is not null)
        {
         
            _context.Entry(ceza).Property(c => c.RowVersion).OriginalValue = rowVersion;
        }

        return ceza;
    }

    private async Task<Sonuc> DurumDegistirAsync(TrafikCezasi ceza, CezaDurumu yeniDurum,
        IslemTipi islemTipi, ClaimsPrincipal kullanici, string? aciklama)
    {
        var oncekiDurum = ceza.Durum;
        ceza.Durum = yeniDurum;

        _context.OnayGecmisleri.Add(new OnayGecmisi
        {
            TrafikCezasiId = ceza.Id,
            KullaniciId = kullanici.FindFirstValue(ClaimTypes.NameIdentifier)!,
            IslemTarihi = DateTime.Now,
            IslemTipi = islemTipi,
            Aciklama = aciklama,
            OncekiDurum = oncekiDurum,
            YeniDurum = yeniDurum
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Sonuc.Hatali(GuncelOlmayanKayitMesaji);
        }

        return Sonuc.Basarili();
    }
}