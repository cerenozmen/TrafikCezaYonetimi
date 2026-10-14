using Microsoft.AspNetCore.Identity;
using TrafikCezaYonetimi.Web.Entities.Enums;

namespace TrafikCezaYonetimi.Web.Entities;

public class OnayGecmisi
{
    public int Id { get; set; }

    public int TrafikCezasiId { get; set; }
    public TrafikCezasi TrafikCezasi { get; set; } = null!;

    public string KullaniciId { get; set; } = string.Empty;
    public IdentityUser Kullanici { get; set; } = null!;

    public DateTime IslemTarihi { get; set; }
    public IslemTipi IslemTipi { get; set; }
    public string? Aciklama { get; set; }

    // "Oluşturuldu" kaydında önceki durum olmaz, bu yüzden boş bırakılabilir (?)
    public CezaDurumu? OncekiDurum { get; set; }
    public CezaDurumu YeniDurum { get; set; }
}