using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Data;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;

public class CezaService : ICezaService
{
    private readonly ApplicationDbContext _context;

    public CezaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TrafikCezasi>> TumunuGetirAsync()
    {
        return await _context.TrafikCezalari
            .AsNoTracking()
            .Include(c => c.Arac)
            .OrderByDescending(c => c.OlusturmaTarihi)
            .ToListAsync();
    }

    public async Task<TrafikCezasi?> DetayGetirAsync(int id)
    {
        return await _context.TrafikCezalari
            .AsNoTracking()
            .Include(c => c.Arac)
            .Include(c => c.OlusturanKullanici)
            .Include(c => c.OnayGecmisleri)
                .ThenInclude(g => g.Kullanici)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Sonuc> EkleAsync(CezaFormViewModel form, string kullaniciId)
    {
        var dogrulama = await DogrulaAsync(form, haricTutulacakCezaId: null);
        if (!dogrulama.BasariliMi)
            return dogrulama;

        var simdi = DateTime.Now;

        var ceza = new TrafikCezasi
        {
            AracId = form.AracId!.Value,
            TutanakNo = TutanakNoNormalizeEt(form.TutanakNo),
            CezaTarihi = form.CezaTarihi!.Value,
            Tutar = form.Tutar!.Value,
            IhlalAciklamasi = form.IhlalAciklamasi.Trim(),
            Durum = CezaDurumu.Yeni,
            OlusturanKullaniciId = kullaniciId,
            OlusturmaTarihi = simdi
        };

        ceza.OnayGecmisleri.Add(new OnayGecmisi
        {
            KullaniciId = kullaniciId,
            IslemTarihi = simdi,
            IslemTipi = IslemTipi.Olusturuldu,
            OncekiDurum = null,
            YeniDurum = CezaDurumu.Yeni,
            Aciklama = "Ceza kaydı oluşturuldu."
        });

        _context.TrafikCezalari.Add(ceza);
        await _context.SaveChangesAsync();

        return Sonuc.Basarili();
    }

    public async Task<Sonuc> GuncelleAsync(CezaFormViewModel form, string kullaniciId)
    {
        var ceza = await _context.TrafikCezalari.FirstOrDefaultAsync(c => c.Id == form.Id);

        if (ceza is null)
            return Sonuc.Hatali("Ceza kaydı bulunamadı.");

        if (ceza.OlusturanKullaniciId != kullaniciId)
            return Sonuc.Hatali("Bu cezayı yalnızca oluşturan kişi düzenleyebilir.");

        if (!ceza.DuzenlenebilirMi())
            return Sonuc.Hatali("Onay sürecindeki veya tamamlanmış cezalar düzenlenemez.");

        var dogrulama = await DogrulaAsync(form, ceza.Id);
        if (!dogrulama.BasariliMi)
            return dogrulama;

        ceza.AracId = form.AracId!.Value;
        ceza.TutanakNo = TutanakNoNormalizeEt(form.TutanakNo);
        ceza.CezaTarihi = form.CezaTarihi!.Value;
        ceza.Tutar = form.Tutar!.Value;
        ceza.IhlalAciklamasi = form.IhlalAciklamasi.Trim();

        await _context.SaveChangesAsync();
        return Sonuc.Basarili();
    }

    private async Task<Sonuc> DogrulaAsync(CezaFormViewModel form, int? haricTutulacakCezaId)
    {
        if (!await _context.Araclar.AnyAsync(a => a.Id == form.AracId))
            return Sonuc.Hatali("Seçilen araç bulunamadı.");

        if (form.CezaTarihi > DateTime.Now)
            return Sonuc.Hatali("Ceza tarihi gelecekte olamaz.");

        var tutanakNo = TutanakNoNormalizeEt(form.TutanakNo);
        var sorgu = _context.TrafikCezalari.Where(c => c.TutanakNo == tutanakNo);

        if (haricTutulacakCezaId.HasValue)
            sorgu = sorgu.Where(c => c.Id != haricTutulacakCezaId.Value);

        if (await sorgu.AnyAsync())
            return Sonuc.Hatali($"{tutanakNo} numaralı tutanak zaten kayıtlı.");

        return Sonuc.Basarili();
    }

    private static string TutanakNoNormalizeEt(string tutanakNo)
    {
        return tutanakNo.Trim().ToUpperInvariant();
    }
}