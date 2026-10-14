using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimi.Web.Entities.Enums;

public enum AracTipi
{
    // AracTipi enum'ı, araçların tiplerini temsil eder ve her tip için bir görünür ad sağlar.
    
    [Display(Name = "Binek")]
    Binek = 1,

    [Display(Name = "Çekici")]
    Cekici = 2,

    [Display(Name = "Dorse")]
    Dorse = 3,

    [Display(Name = "Kiralık Araç")]
    KiralikArac = 4
}