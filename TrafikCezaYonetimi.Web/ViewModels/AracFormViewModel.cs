using System.ComponentModel.DataAnnotations;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.ViewModels;

public class AracFormViewModel
{
    [Display(Name = "Plaka")]
    [Required(ErrorMessage = "Plaka zorunludur.")]
    [StringLength(15, ErrorMessage = "Plaka en fazla 15 karakter olabilir.")]
    [RegularExpression(@"^(0[1-9]|[1-7][0-9]|8[01])\s*[A-Za-z]{1,3}\s*[0-9]{2,4}$",
        ErrorMessage = "Geçerli bir plaka giriniz. Örnek: 34 ABC 123")]
    public string Plaka { get; set; } = string.Empty;

    [Display(Name = "Araç Tipi")]
    [Required(ErrorMessage = "Araç tipi seçiniz.")]
    public AracTipi? AracTipi { get; set; }

    [Display(Name = "Marka")]
    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(50, ErrorMessage = "Marka en fazla 50 karakter olabilir.")]
    public string Marka { get; set; } = string.Empty;

    [Display(Name = "Model")]
    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(50, ErrorMessage = "Model en fazla 50 karakter olabilir.")]
    public string Model { get; set; } = string.Empty;
}