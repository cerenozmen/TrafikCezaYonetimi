using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.ViewModels;
// CezaListeViewModel, ceza listesi sayfasında kullanıcıya gösterilecek cezaları ve filtreleme seçeneklerini temsil eder. Bu model, cezaların listesini ve sadece bekleyen cezaların gösterilip gösterilmeyeceğini içerir.      
public class CezaListeViewModel
{
    public List<TrafikCezasi> Cezalar { get; set; } = new();
    public bool SadeceBekleyenler { get; set; }
}