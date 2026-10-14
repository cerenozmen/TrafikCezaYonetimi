using System.Security.Claims;
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

// TumunuGetirAsync, veritabanındaki tüm cezaları getirir ve oluşturulma tarihlerine göre sıralar.
    public async Task<List<TrafikCezasi>> TumunuGetirAsync()
    {
        return await _context.TrafikCezalari
            .AsNoTracking()
            .Include(c => c.Arac)
            .OrderByDescending(c => c.OlusturmaTarihi)
            .ToListAsync();
    }

// BekleyenleriGetirAsync, kullanıcının onaylayabileceği veya kendisinin oluşturduğu ve onaya gönderilebilecek cezaları getirir.    
    public async Task<List<TrafikCezasi>> BekleyenleriGetirAsync(ClaimsPrincipal kullanici)
    {
        var kullaniciId = kullanici.FindFirstValue(ClaimTypes.NameIdentifier);
        var kararDurumlari = OnayAkisi.KararVerebilecegiDurumlar(kullanici);
        var gonderilebilirDurumlar = OnayAkisi.OnayaGonderilebilirDurumlar;

        return await _context.TrafikCezalari
            .AsNoTracking()
            .Include(c => c.Arac)
            .Where(c => kararDurumlari.Contains(c.Durum)
                        || (c.OlusturanKullaniciId == kullaniciId && gonderilebilirDurumlar.Contains(c.Durum)))
            .OrderBy(c => c.OlusturmaTarihi)
            .ToListAsync();
    }
// DurumSayilariGetirAsync, her ceza durumunun sayısını getirir. 
    public async Task<Dictionary<CezaDurumu, int>> DurumSayilariGetirAsync()
    {
        return await _context.TrafikCezalari
            .GroupBy(c => c.Durum)
            .Select(g => new { Durum = g.Key, Sayi = g.Count() })
            .ToDictionaryAsync(x => x.Durum, x => x.Sayi);
    }

// DetayGetirAsync, belirli bir ceza kaydının detaylarını getirir. Bu detaylar arasında araç bilgisi, oluşturan kullanıcı ve onay geçmişi bulunur.
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
// EkleAsync, yeni bir ceza kaydı ekler. Form verilerini doğrular ve ceza kaydını oluşturur. Oluşturulan ceza kaydı için onay geçmişi de eklenir.
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
// GuncelleAsync, mevcut bir ceza kaydını günceller. Form verilerini doğrular ve ceza kaydını günceller. 
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
// DogrulaAsync, ceza form verilerini doğrular. Araç varlığını, ceza tarihinin geçerliliğini ve tutanak numarasının benzersizliğini kontrol eder.
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

// TutanakNoNormalizeEt, tutanak numarasını normalize eder; boşlukları kaldırır ve büyük harfe çevirir.
    private static string TutanakNoNormalizeEt(string tutanakNo)
    {
        return tutanakNo.Trim().ToUpperInvariant();
    }
}