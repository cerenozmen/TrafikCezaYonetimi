using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.ViewModels;
// AnaSayfaViewModel, ana sayfada kullanıcıya gösterilecek özet bilgileri ve istatistikleri temsil eder. Bu model, ceza durumlarına göre sayıları ve bekleyen cezaların sayısını içerir.    
public class AnaSayfaViewModel
{
    public Dictionary<CezaDurumu, int> DurumSayilari { get; set; } = new();
    public int BekleyenSayisi { get; set; }
}