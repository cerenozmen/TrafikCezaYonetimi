using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.ViewModels;

public class CezaListeViewModel
{
    public List<TrafikCezasi> Cezalar { get; set; } = new();
    public bool SadeceBekleyenler { get; set; }
}