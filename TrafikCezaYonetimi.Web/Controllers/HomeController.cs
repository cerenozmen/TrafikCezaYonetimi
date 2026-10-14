using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TrafikCezaYonetimi.Web.Models;
using TrafikCezaYonetimi.Web.Services;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICezaService _cezaService;

    public HomeController(ICezaService cezaService)
    {
        _cezaService = cezaService;
    }
    // Ana sayfa, kullanıcı giriş yaptıysa ceza durum sayıları ve bekleyen ceza sayısını gösterir
    public async Task<IActionResult> Index()
    {
        var model = new AnaSayfaViewModel();

        if (User.Identity?.IsAuthenticated == true)
        {
            model.DurumSayilari = await _cezaService.DurumSayilariGetirAsync();
            model.BekleyenSayisi = (await _cezaService.BekleyenleriGetirAsync(User)).Count;
        }

        return View(model);
    }

    // Yakalanmamış hatalarda (sadece Production ortamında) gösterilir
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // 404 gibi durum kodlarında gösterilir
    public IActionResult DurumKodu(int kod)
    {
        return View(kod);
    }

    // Rolü yetmeyen kullanıcı yetkisiz bir sayfaya girmeye çalışınca gösterilir
    public IActionResult YetkisizErisim()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View("DurumKodu", StatusCodes.Status403Forbidden);
    }
}