using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafikCezaYonetimi.Web.Services;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Controllers;

[Authorize]
public class AracController : Controller
{
    private readonly IAracService _aracService;

    public AracController(IAracService aracService)
    {
        _aracService = aracService;
    }

    // Araçların listelendiği sayfa
    public async Task<IActionResult> Index()
    {
        var araclar = await _aracService.TumunuGetirAsync();
        return View(araclar);
    }

    // Araç ekleme formunu gösterir

    [HttpGet]
    public IActionResult Ekle()
    {
        return View(new AracFormViewModel());
    }

  // Araç ekleme formunu işler
   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Ekle(AracFormViewModel aracForm)
{
    if (!ModelState.IsValid)
        return View(aracForm);

    var sonuc = await _aracService.EkleAsync(aracForm);
    if (!sonuc.BasariliMi)
    {
        ModelState.AddModelError(nameof(aracForm.Plaka), sonuc.HataMesaji!);
        return View(aracForm);
    }

    TempData["Basari"] = "Araç başarıyla eklendi.";
    return RedirectToAction(nameof(Index));
}

}
