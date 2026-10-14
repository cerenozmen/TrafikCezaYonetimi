namespace TrafikCezaYonetimi.Web.Common;

public class Sonuc
{
    // İşlemin başarılı olup olmadığını gösterir
    // Hata mesajı varsa, işlemin neden başarısız olduğunu açıklar
    public bool BasariliMi { get; }
    public string? HataMesaji { get; }

    private Sonuc(bool basariliMi, string? hataMesaji)
    {
        BasariliMi = basariliMi;
        HataMesaji = hataMesaji;
    }

    public static Sonuc Basarili() => new(true, null);
    public static Sonuc Hatali(string hataMesaji) => new(false, hataMesaji);
}