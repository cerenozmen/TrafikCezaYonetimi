using System.Security.Claims;
using TrafikCezaYonetimi.Web.Constants;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Services;

public static class OnayAkisi
{
    // Hangi durumda hangi rol karar verir?
    private static readonly Dictionary<CezaDurumu, string> KararVerecekRoller = new()
    {
        [CezaDurumu.YoneticiOnayinda] = Roller.Yonetici,
        [CezaDurumu.FinansOnayinda] = Roller.Finans
    };

    // Onay verilirse ceza hangi duruma geçer?
    private static readonly Dictionary<CezaDurumu, CezaDurumu> OnaySonrasiDurumlar = new()
    {
        [CezaDurumu.YoneticiOnayinda] = CezaDurumu.FinansOnayinda,
        [CezaDurumu.FinansOnayinda] = CezaDurumu.Tamamlandi
    };

    // Hangi durumlardaki ceza onaya gönderilebilir?
    public static readonly CezaDurumu[] OnayaGonderilebilirDurumlar =
    {
        CezaDurumu.Yeni,
        CezaDurumu.Reddedildi
    };

    public static bool KararVerebilirMi(TrafikCezasi ceza, ClaimsPrincipal kullanici)
    {
        return KararVerecekRoller.TryGetValue(ceza.Durum, out var rol)
               && kullanici.IsInRole(rol);
    }

    public static bool OnayaGonderebilirMi(TrafikCezasi ceza, ClaimsPrincipal kullanici)
    {
        return OnayaGonderilebilirDurumlar.Contains(ceza.Durum)
               && ceza.OlusturanKullaniciId == kullanici.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static CezaDurumu OnaySonrasiDurum(CezaDurumu mevcutDurum)
    {
        return OnaySonrasiDurumlar[mevcutDurum];
    }

    // Kullanıcının karar verebileceği durumlar (örneğin Yönetici için: YoneticiOnayinda)
    public static List<CezaDurumu> KararVerebilecegiDurumlar(ClaimsPrincipal kullanici)
    {
        return KararVerecekRoller
            .Where(kayit => kullanici.IsInRole(kayit.Value))
            .Select(kayit => kayit.Key)
            .ToList();
    }
}