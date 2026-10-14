using System.Security.Claims;
using TrafikCezaYonetimi.Web.Constants;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Services;

public static class OnayAkisi
{
    // CezaDurumu enum'ı ile Roller arasında bir eşleme yapar. Örneğin, YoneticiOnayinda durumunda karar verecek rol Yonetici'dir.  
    private static readonly Dictionary<CezaDurumu, string> KararVerecekRoller = new()
    {
        [CezaDurumu.YoneticiOnayinda] = Roller.Yonetici,
        [CezaDurumu.FinansOnayinda] = Roller.Finans
    };

    // CezaDurumu enum'ı ile bir sonraki durum arasındaki geçişleri tanımlar. Örneğin, YoneticiOnayinda durumundan sonra FinansOnayinda durumuna geçilir.   
    private static readonly Dictionary<CezaDurumu, CezaDurumu> OnaySonrasiDurumlar = new()
    {
        [CezaDurumu.YoneticiOnayinda] = CezaDurumu.FinansOnayinda,
        [CezaDurumu.FinansOnayinda] = CezaDurumu.Tamamlandi
    };

    //  OnayaGonderilebilirDurumlar, bir cezanın onaya gönderilebileceği durumları tanımlar. Örneğin, Yeni veya Reddedildi durumundaki cezalar onaya gönderilebilir.    
    public static readonly CezaDurumu[] OnayaGonderilebilirDurumlar =
    {
        CezaDurumu.Yeni,
        CezaDurumu.Reddedildi
    };

// KararVerebilirMi, kullanıcının belirli bir ceza kaydı üzerinde karar verip veremeyeceğini kontrol eder. Kullanıcının rolü ve cezanın durumu dikkate alınır.  
    public static bool KararVerebilirMi(TrafikCezasi ceza, ClaimsPrincipal kullanici)
    {
        return KararVerecekRoller.TryGetValue(ceza.Durum, out var rol)
               && kullanici.IsInRole(rol);
    }

// OnayaGonderebilirMi, kullanıcının belirli bir ceza kaydını onaya gönderip gönderemeyeceğini kontrol eder. Kullanıcının cezanın oluşturucusu olup olmadığı ve cezanın durumu dikkate alınır.  
    public static bool OnayaGonderebilirMi(TrafikCezasi ceza, ClaimsPrincipal kullanici)
    {
        return OnayaGonderilebilirDurumlar.Contains(ceza.Durum)
               && ceza.OlusturanKullaniciId == kullanici.FindFirstValue(ClaimTypes.NameIdentifier);
    }

// OnaySonrasiDurum, mevcut ceza durumuna göre bir sonraki durumu döndürür. Örneğin, YoneticiOnayinda durumundan sonra FinansOnayinda durumuna geçilir. 
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