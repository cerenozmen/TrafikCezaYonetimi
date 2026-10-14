using System.Globalization;

namespace TrafikCezaYonetimi.Web.Common;

public static class FormatExtensions
{
    // Verilen decimal değeri Türk Lirası formatında döndürür
    private static readonly CultureInfo TurkceKultur = new("tr-TR");

    public static string ParaFormatla(this decimal tutar)
    {
        return tutar.ToString("N2", TurkceKultur) + " ₺";
    }
}