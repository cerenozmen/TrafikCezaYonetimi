using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.ViewModels;

public class AnaSayfaViewModel
{
    public Dictionary<CezaDurumu, int> DurumSayilari { get; set; } = new();
    public int BekleyenSayisi { get; set; }
}