using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Data;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Services;

public class OnayService : IOnayService
{
    // GuncelOlmayanKayitMesaji, ceza kaydının başka bir kullanıcı tarafından değiştirildiğini belirten hata mesajıdır. Kullanıcıya sayfayı yenileyip tekrar denemesi gerektiğini bildirir. 
    private const string GuncelOlmayanKayitMesaji =
        "Bu ceza siz sayfayı açtıktan sonra başka bir kullanıcı tarafından değiştirildi. Lütfen sayfayı yenileyip tekrar deneyin.";

    private const int AciklamaMaksimumUzunluk = 500;

    private readonly ApplicationDbContext _context;

    public OnayService(ApplicationDbContext context)
    {
        _context = context;
    }

// OnayaGonderAsync, belirli bir ceza kaydını onaya gönderir. Kullanıcının yetkisi ve cezanın durumu kontrol edilir. Eğer ceza zaten başka bir kullanıcı tarafından değiştirilmişse hata döner. 
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
// OnaylaAsync, belirli bir ceza kaydını onaylar. Kullanıcının yetkisi ve cezanın durumu kontrol edilir. Eğer ceza zaten başka bir kullanıcı tarafından değiştirilmişse hata döner. 
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

// ReddetAsync, belirli bir ceza kaydını reddeder. Kullanıcının yetkisi ve cezanın durumu kontrol edilir. Ret nedeni zorunludur ve maksimum uzunluk sınırına tabidir. Eğer ceza zaten başka bir kullanıcı tarafından değiştirilmişse hata döner.    
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
// CezaGetirAsync, belirli bir ceza kaydını getirir ve eşzamanlılık kontrolü için RowVersion değerini ayarlar. Eğer ceza bulunamazsa null döner.    
    private async Task<TrafikCezasi?> CezaGetirAsync(int cezaId, byte[] rowVersion)
    {
        var ceza = await _context.TrafikCezalari.FirstOrDefaultAsync(c => c.Id == cezaId);

        if (ceza is not null)
        {
         
            _context.Entry(ceza).Property(c => c.RowVersion).OriginalValue = rowVersion;
        }

        return ceza;
    }

// DurumDegistirAsync, ceza kaydının durumunu değiştirir ve onay geçmişine bir kayıt ekler. Eşzamanlılık kontrolü yapılır ve eğer başka bir kullanıcı tarafından değişiklik yapılmışsa hata döner.  
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