using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.ViewModels;
// CezaDetayViewModel, ceza detay sayfasında kullanıcıya gösterilecek bilgileri ve yetkilendirme durumlarını temsil eder. Bu model, ceza bilgilerini ve kullanıcının ceza üzerinde yapabileceği işlemleri içerir.   
public class CezaDetayViewModel
{
    public TrafikCezasi Ceza { get; set; } = null!;
    public bool DuzenleyebilirMi { get; set; }
    public bool OnayaGonderebilirMi { get; set; }
    public bool KararVerebilirMi { get; set; }

    public bool IslemYapabilirMi => OnayaGonderebilirMi || KararVerebilirMi;
}