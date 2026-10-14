using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Entities;

namespace TrafikCezaYonetimi.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Arac> Araclar => Set<Arac>();
    public DbSet<TrafikCezasi> TrafikCezalari => Set<TrafikCezasi>();
    public DbSet<OnayGecmisi> OnayGecmisleri => Set<OnayGecmisi>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}