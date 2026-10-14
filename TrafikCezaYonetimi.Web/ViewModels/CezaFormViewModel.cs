using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrafikCezaYonetimi.Web.ViewModels;
// CezaFormViewModel, ceza ekleme ve düzenleme işlemlerinde kullanılan form verilerini temsil eder. Bu model, ceza bilgilerini toplar ve doğrulama kurallarını içerir.  
public class CezaFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Araç")]
    [Required(ErrorMessage = "Araç seçiniz.")]
    public int? AracId { get; set; }

    [Display(Name = "Tutanak No")]
    [Required(ErrorMessage = "Tutanak numarası zorunludur.")]
    [StringLength(30, ErrorMessage = "Tutanak numarası en fazla 30 karakter olabilir.")]
    public string TutanakNo { get; set; } = string.Empty;

    [Display(Name = "Ceza Tarihi")]
    [Required(ErrorMessage = "Ceza tarihi zorunludur.")]
    public DateTime? CezaTarihi { get; set; }

    [Display(Name = "Tutar (₺)")]
    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(0.01, 1000000, ErrorMessage = "Tutar 0,01 ile 1.000.000 ₺ arasında olmalıdır.")]
    public decimal? Tutar { get; set; }

    [Display(Name = "İhlal Açıklaması")]
    [Required(ErrorMessage = "İhlal açıklaması zorunludur.")]
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    public string IhlalAciklamasi { get; set; } = string.Empty;

    [ValidateNever]
    public List<SelectListItem> AracSecenekleri { get; set; } = new();
}