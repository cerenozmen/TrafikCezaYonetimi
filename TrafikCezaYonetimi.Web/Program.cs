using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Data;
using TrafikCezaYonetimi.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Kimlik ve roller
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/YetkisizErisim";
});

builder.Services.AddControllersWithViews();

// Uygulama servisleri
builder.Services.AddScoped<IAracService, AracService>();
builder.Services.AddScoped<ICezaService, CezaService>();
builder.Services.AddScoped<IOnayService, OnayService>();

var app = builder.Build();

// Başlangıç verisi (roller ve test kullanıcıları)
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// Hata yönetimi
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/DurumKodu", "?kod={0}");

// Sayı ve tarihlerin dilden bağımsız okunması
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
    SupportedCultures = new List<CultureInfo> { CultureInfo.InvariantCulture },
    SupportedUICultures = new List<CultureInfo> { CultureInfo.InvariantCulture }
});

app.UseHttpsRedirection();

// Kullanıcılar yönetim tarafından oluşturulur; dışarıdan kayıt kapalı
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    await next();
});

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();