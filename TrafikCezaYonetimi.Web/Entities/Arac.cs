using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Entities;

public class Arac
{
    // Arac entity'si, araçların temel bilgilerini ve ilişkili trafik cezalarını temsil eder
    public int Id { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public AracTipi AracTipi { get; set; }
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime OlusturmaTarihi { get; set; }

    public ICollection<TrafikCezasi> Cezalar { get; set; } = new List<TrafikCezasi>();
}