using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Constants;
using TrafikCezaYonetimi.Web.Services;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Controllers;

[Authorize]
public class CezaController : Controller
{
    private readonly ICezaService _cezaService;
    private readonly IAracService _aracService;
    private readonly IOnayService _onayService;

    public CezaController(ICezaService cezaService, IAracService aracService, IOnayService onayService)
    {
        _cezaService = cezaService;
        _aracService = aracService;
        _onayService = onayService;
    }

    private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

   public async Task<IActionResult> Index(string? filtre)
{
    var sadeceBekleyenler = filtre == "bekleyen";

    var cezalar = sadeceBekleyenler
        ? await _cezaService.BekleyenleriGetirAsync(User)
        : await _cezaService.TumunuGetirAsync();

    return View(new CezaListeViewModel
    {
        Cezalar = cezalar,
        SadeceBekleyenler = sadeceBekleyenler
    });
}

    public async Task<IActionResult> Detay(int id)
    {
        var ceza = await _cezaService.DetayGetirAsync(id);
        if (ceza is null)
            return NotFound();

        var detay = new CezaDetayViewModel
        {
            Ceza = ceza,
            DuzenleyebilirMi = ceza.OlusturanKullaniciId == KullaniciId && ceza.DuzenlenebilirMi(),
            OnayaGonderebilirMi = OnayAkisi.OnayaGonderebilirMi(ceza, User),
            KararVerebilirMi = OnayAkisi.KararVerebilirMi(ceza, User)
        };

        return View(detay);
    }

    // ---------- Ekleme ve düzenleme ----------

    [HttpGet]
    [Authorize(Roles = Roller.Personel)]
    public async Task<IActionResult> Ekle()
    {
        var form = new CezaFormViewModel { CezaTarihi = DateTime.Today };
        await AracSecenekleriniDoldurAsync(form);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roller.Personel)]
    public async Task<IActionResult> Ekle(CezaFormViewModel cezaForm)
    {
        if (!ModelState.IsValid)
        {
            await AracSecenekleriniDoldurAsync(cezaForm);
            return View(cezaForm);
        }

        var sonuc = await _cezaService.EkleAsync(cezaForm, KullaniciId);
        if (!sonuc.BasariliMi)
        {
            ModelState.AddModelError(string.Empty, sonuc.HataMesaji!);
            await AracSecenekleriniDoldurAsync(cezaForm);
            return View(cezaForm);
        }

        TempData["Basari"] = "Ceza kaydı oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = Roller.Personel)]
    public async Task<IActionResult> Duzenle(int id)
    {
        var ceza = await _cezaService.DetayGetirAsync(id);
        if (ceza is null)
            return NotFound();

        if (ceza.OlusturanKullaniciId != KullaniciId || !ceza.DuzenlenebilirMi())
        {
            TempData["Hata"] = "Bu ceza kaydını düzenleyemezsiniz.";
            return RedirectToAction(nameof(Detay), new { id });
        }

        var form = new CezaFormViewModel
        {
            Id = ceza.Id,
            AracId = ceza.AracId,
            TutanakNo = ceza.TutanakNo,
            CezaTarihi = ceza.CezaTarihi,
            Tutar = ceza.Tutar,
            IhlalAciklamasi = ceza.IhlalAciklamasi
        };

        await AracSecenekleriniDoldurAsync(form);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roller.Personel)]
    public async Task<IActionResult> Duzenle(CezaFormViewModel cezaForm)
    {
        if (!ModelState.IsValid)
        {
            await AracSecenekleriniDoldurAsync(cezaForm);
            return View(cezaForm);
        }

        var sonuc = await _cezaService.GuncelleAsync(cezaForm, KullaniciId);
        if (!sonuc.BasariliMi)
        {
            ModelState.AddModelError(string.Empty, sonuc.HataMesaji!);
            await AracSecenekleriniDoldurAsync(cezaForm);
            return View(cezaForm);
        }

        TempData["Basari"] = "Ceza kaydı güncellendi.";
        return RedirectToAction(nameof(Detay), new { id = cezaForm.Id });
    }

    // ---------- Onay akışı ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnayaGonder(int id, byte[]? rowVersion)
    {
        var sonuc = await _onayService.OnayaGonderAsync(id, rowVersion ?? Array.Empty<byte>(), User);
        SonucMesajiniAyarla(sonuc, "Ceza onaya gönderildi.");
        return RedirectToAction(nameof(Detay), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Onayla(int id, byte[]? rowVersion, string? aciklama)
    {
        var sonuc = await _onayService.OnaylaAsync(id, rowVersion ?? Array.Empty<byte>(), User, aciklama);
        SonucMesajiniAyarla(sonuc, "Ceza onaylandı.");
        return RedirectToAction(nameof(Detay), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reddet(int id, byte[]? rowVersion, string? retNedeni)
    {
        var sonuc = await _onayService.ReddetAsync(id, rowVersion ?? Array.Empty<byte>(), User, retNedeni);
        SonucMesajiniAyarla(sonuc, "Ceza reddedildi.");
        return RedirectToAction(nameof(Detay), new { id });
    }

    // ---------- Yardımcılar ----------

    private void SonucMesajiniAyarla(Sonuc sonuc, string basariMesaji)
    {
        if (sonuc.BasariliMi)
            TempData["Basari"] = basariMesaji;
        else
            TempData["Hata"] = sonuc.HataMesaji;
    }

    private async Task AracSecenekleriniDoldurAsync(CezaFormViewModel form)
    {
        var araclar = await _aracService.TumunuGetirAsync();
        form.AracSecenekleri = araclar
            .Select(a => new SelectListItem($"{a.Plaka} - {a.Marka} {a.Model}", a.Id.ToString()))
            .ToList();
    }
}