using System.Security.Claims;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;
// ICezaService, trafik cezalarıyla ilgili işlemleri tanımlayan bir servis arayüzüdür. Bu arayüz, cezaların tümünü getirme, bekleyen cezaları getirme, ceza detaylarını alma ve ceza ekleme/güncelleme gibi temel işlemleri içerir. 
public interface ICezaService
{
    Task<List<TrafikCezasi>> TumunuGetirAsync();
    Task<List<TrafikCezasi>> BekleyenleriGetirAsync(ClaimsPrincipal kullanici);
    Task<Dictionary<CezaDurumu, int>> DurumSayilariGetirAsync();
    Task<TrafikCezasi?> DetayGetirAsync(int id);
    Task<Sonuc> EkleAsync(CezaFormViewModel form, string kullaniciId);
    Task<Sonuc> GuncelleAsync(CezaFormViewModel form, string kullaniciId);
}