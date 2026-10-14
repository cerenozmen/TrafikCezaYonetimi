using System.Security.Claims;
using TrafikCezaYonetimi.Web.Common;

namespace TrafikCezaYonetimi.Web.Services;

public interface IOnayService
{
    Task<Sonuc> OnayaGonderAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici);
    Task<Sonuc> OnaylaAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? aciklama);
    Task<Sonuc> ReddetAsync(int cezaId, byte[] rowVersion, ClaimsPrincipal kullanici, string? retNedeni);
}