using System.ComponentModel.DataAnnotations;

namespace TrafikCezaYonetimi.Web.Entities.Enums;

public enum IslemTipi
{
    [Display(Name = "Oluşturuldu")]
    Olusturuldu = 1,

    [Display(Name = "Onaya Gönderildi")]
    OnayaGonderildi = 2,

    [Display(Name = "Onaylandı")]
    Onaylandi = 3,

    [Display(Name = "Reddedildi")]
    Reddedildi = 4
}