using System.Security.Claims;
using TrafikCezaYonetimi.Web.Common;

namespace TrafikCezaYonetimi.Web.Services;
// IOnayService, trafik cezalarının onay süreçleriyle ilgili işlemleri tanımlayan bir servis arayüzüdür. Bu arayüz, cezaların onaya gönderilmesi, onaylanması ve reddedilmesi gibi temel işlemleri içerir.  
public interface IOnayService
{
    Task<Sonuc> OnayaGonderAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici);
    Task<Sonuc> OnaylaAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? aciklama);
    Task<Sonuc> ReddetAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? retNedeni);
}