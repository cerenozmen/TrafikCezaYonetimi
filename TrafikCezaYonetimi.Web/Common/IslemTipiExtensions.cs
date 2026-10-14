using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Common;

public static class IslemTipiExtensions
{
    public static string BadgeSinifi(this IslemTipi islemTipi)
    {
        return islemTipi switch
        {
            IslemTipi.Olusturuldu => "bg-secondary",
            IslemTipi.OnayaGonderildi => "bg-primary",
            IslemTipi.Onaylandi => "bg-success",
            IslemTipi.Reddedildi => "bg-danger",
            _ => "bg-secondary"
        };
    }
}