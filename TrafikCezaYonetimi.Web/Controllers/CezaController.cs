using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafikCezaYonetimi.Web.Constants;
using TrafikCezaYonetimi.Web.Services;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Controllers;

[Authorize]
public class CezaController : Controller
{
    private readonly ICezaService _cezaService;
    private readonly IAracService _aracService;

    public CezaController(ICezaService cezaService, IAracService aracService)
    {
        _cezaService = cezaService;
        _aracService = aracService;
    }

    private string KullaniciId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public async Task<IActionResult> Index()
    {
        var cezalar = await _cezaService.TumunuGetirAsync();
        return View(cezalar);
    }

    public async Task<IActionResult> Detay(int id)
    {
        var ceza = await _cezaService.DetayGetirAsync(id);
        if (ceza is null)
            return NotFound();

        var detay = new CezaDetayViewModel
        {
            Ceza = ceza,
            DuzenleyebilirMi = ceza.OlusturanKullaniciId == KullaniciId && ceza.DuzenlenebilirMi()
        };

        return View(detay);
    }

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

    private async Task AracSecenekleriniDoldurAsync(CezaFormViewModel form)
    {
        var araclar = await _aracService.TumunuGetirAsync();
        form.AracSecenekleri = araclar
            .Select(a => new SelectListItem($"{a.Plaka} - {a.Marka} {a.Model}", a.Id.ToString()))
            .ToList();
    }
}