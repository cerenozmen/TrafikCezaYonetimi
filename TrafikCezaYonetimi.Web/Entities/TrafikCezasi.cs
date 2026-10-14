using Microsoft.AspNetCore.Identity;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Entities;

public class TrafikCezasi
{
    public int Id { get; set; }
    public string TutanakNo { get; set; } = string.Empty;
    public DateTime CezaTarihi { get; set; }
    public decimal Tutar { get; set; }
    public string IhlalAciklamasi { get; set; } = string.Empty;
    public CezaDurumu Durum { get; set; } = CezaDurumu.Yeni;

    public int AracId { get; set; }
    public Arac Arac { get; set; } = null!;

    public string OlusturanKullaniciId { get; set; } = string.Empty;
    public IdentityUser OlusturanKullanici { get; set; } = null!;
    public DateTime OlusturmaTarihi { get; set; }

    // Eşzamanlı onay çakışmalarını önlemek için
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<OnayGecmisi> OnayGecmisleri { get; set; } = new List<OnayGecmisi>();
    public bool DuzenlenebilirMi() => Durum is CezaDurumu.Yeni or CezaDurumu.Reddedildi;
}