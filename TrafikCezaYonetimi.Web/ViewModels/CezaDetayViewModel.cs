using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.ViewModels;

public class CezaDetayViewModel
{
    public TrafikCezasi Ceza { get; set; } = null!;
    public bool DuzenleyebilirMi { get; set; }
}