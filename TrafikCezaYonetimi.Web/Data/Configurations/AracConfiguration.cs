using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.Data.Configurations;

public class AracConfiguration : IEntityTypeConfiguration<Arac>
{
    public void Configure(EntityTypeBuilder<Arac> builder)
    {
        builder.ToTable("Araclar");

        builder.Property(a => a.Plaka).IsRequired().HasMaxLength(15);
        builder.HasIndex(a => a.Plaka).IsUnique();

        builder.Property(a => a.Marka).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Model).IsRequired().HasMaxLength(50);

        builder.Property(a => a.AracTipi)
               .HasConversion<string>()
               .HasMaxLength(20);
    }
}