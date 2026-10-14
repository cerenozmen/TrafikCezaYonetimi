using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Constants;

namespace TrafikCezaYonetimi.Web.Data;

public static class SeedData
{
    private const string VarsayilanSifre = "Test123!";

   // Veritabanını başlatır, gerekli tabloları oluşturur ve varsayılan kullanıcıları ve rolleri ekler
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var rol in Roller.Tumu)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        await KullaniciOlusturAsync(userManager, "personel@test.com", Roller.Personel);
        await KullaniciOlusturAsync(userManager, "yonetici@test.com", Roller.Yonetici);
        await KullaniciOlusturAsync(userManager, "finans@test.com", Roller.Finans);
    }

    private static async Task KullaniciOlusturAsync(
        UserManager<IdentityUser> userManager, string email, string rol)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var kullanici = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var sonuc = await userManager.CreateAsync(kullanici, VarsayilanSifre);
        if (!sonuc.Succeeded)
        {
            var hatalar = string.Join(", ", sonuc.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Seed kullanıcısı oluşturulamadı ({email}): {hatalar}");
        }

        await userManager.AddToRoleAsync(kullanici, rol);
    }
}