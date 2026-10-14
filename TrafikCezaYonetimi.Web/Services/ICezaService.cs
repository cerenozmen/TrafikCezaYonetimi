using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;

public interface ICezaService
{
    Task<List<TrafikCezasi>> TumunuGetirAsync();
    Task<TrafikCezasi?> DetayGetirAsync(int id);
    Task<Sonuc> EkleAsync(CezaFormViewModel form, string kullaniciId);
    Task<Sonuc> GuncelleAsync(CezaFormViewModel form, string kullaniciId);
}