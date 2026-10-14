using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.Data.Configurations;

public class OnayGecmisiConfiguration : IEntityTypeConfiguration<OnayGecmisi>
{
    // OnayGecmisi entity'sinin veritabanı yapılandırmasını yapar
    public void Configure(EntityTypeBuilder<OnayGecmisi> builder)
    {
        
        builder.ToTable("OnayGecmisleri");

        builder.Property(g => g.Aciklama).HasMaxLength(500);

        builder.Property(g => g.IslemTipi).HasConversion<string>().HasMaxLength(30);
        builder.Property(g => g.OncekiDurum).HasConversion<string>().HasMaxLength(30);
        builder.Property(g => g.YeniDurum).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(g => g.TrafikCezasi)
               .WithMany(c => c.OnayGecmisleri)
               .HasForeignKey(g => g.TrafikCezasiId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Kullanici)
               .WithMany()
               .HasForeignKey(g => g.KullaniciId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}