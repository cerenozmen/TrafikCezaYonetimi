using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Common;

public static class CezaDurumuExtensions
{
    public static string BadgeSinifi(this CezaDurumu durum)
    {
        return durum switch
        {
            CezaDurumu.Yeni => "bg-secondary",
            CezaDurumu.YoneticiOnayinda => "bg-warning text-dark",
            CezaDurumu.FinansOnayinda => "bg-info text-dark",
            CezaDurumu.Tamamlandi => "bg-success",
            CezaDurumu.Reddedildi => "bg-danger",
            _ => "bg-secondary"
        };
    }
}