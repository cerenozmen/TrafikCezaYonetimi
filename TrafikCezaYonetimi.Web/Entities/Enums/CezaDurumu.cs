using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimi.Web.Entities.Enums;

public enum CezaDurumu
{
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