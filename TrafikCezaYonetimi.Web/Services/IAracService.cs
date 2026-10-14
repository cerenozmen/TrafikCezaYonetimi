using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;
 // IAracService, araçlarla ilgili işlemleri tanımlayan bir servis arayüzüdür. Bu arayüz, araçların tümünü getirme ve yeni araç ekleme gibi temel işlemleri içerir. 
public interface IAracService
{
    Task<List<Arac>> TumunuGetirAsync();
    Task<Sonuc> EkleAsync(AracFormViewModel model);
}