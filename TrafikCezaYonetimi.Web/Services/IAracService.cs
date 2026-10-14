using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;

public interface IAracService
{
    Task<List<Arac>> TumunuGetirAsync();
    Task<Sonuc> EkleAsync(AracFormViewModel model);
}