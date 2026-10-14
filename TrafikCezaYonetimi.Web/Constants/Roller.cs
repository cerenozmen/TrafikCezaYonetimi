namespace TrafikCezaYonetimi.Web.Constants;

public static class Roller
{
    public const string Personel = "Personel";
    public const string Yonetici = "Yonetici";
    public const string Finans = "Finans";

    public static readonly string[] Tumu = { Personel, Yonetici, Finans };

    public static string GorunenAd(string rol) => rol switch
    {
        Yonetici => "Yönetici",
        _ => rol
    };
}