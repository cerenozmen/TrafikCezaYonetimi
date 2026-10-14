using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.Data.Configurations;

public class TrafikCezasiConfiguration : IEntityTypeConfiguration<TrafikCezasi>
{
    public void Configure(EntityTypeBuilder<TrafikCezasi> builder)
    {
        builder.ToTable("TrafikCezalari");

        builder.Property(c => c.TutanakNo).IsRequired().HasMaxLength(30);
        builder.HasIndex(c => c.TutanakNo).IsUnique();

        builder.Property(c => c.Tutar).HasPrecision(18, 2);
        builder.Property(c => c.IhlalAciklamasi).IsRequired().HasMaxLength(500);

        builder.Property(c => c.Durum)
               .HasConversion<string>()
               .HasMaxLength(30);
        builder.HasIndex(c => c.Durum);

        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasOne(c => c.Arac)
               .WithMany(a => a.Cezalar)
               .HasForeignKey(c => c.AracId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.OlusturanKullanici)
               .WithMany()
               .HasForeignKey(c => c.OlusturanKullaniciId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}