using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimi.Web.Entities.Enums;

public enum CezaDurumu
{
    // CezaDurumu enum'ı, trafik cezalarının durumlarını temsil eder ve her durum için bir görünür ad sağlar.
    
    [Display(Name = "Yeni")]
    Yeni = 1,

    [Display(Name = "Yönetici Onayında")]
    YoneticiOnayinda = 2,

    [Display(Name = "Finans Onayında")]
    FinansOnayinda = 3,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 4,

    [Display(Name = "Reddedildi")]
    Reddedildi = 5
}