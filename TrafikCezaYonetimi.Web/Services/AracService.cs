using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TrafikCezaYonetimi.Web.Common;
using TrafikCezaYonetimi.Web.Data;
using TrafikCezaYonetimi.Web.Entities;
using TrafikCezaYonetimi.Web.ViewModels;

namespace TrafikCezaYonetimi.Web.Services;

public class AracService : IAracService
{
    private readonly ApplicationDbContext _context;

    public AracService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Arac>> TumunuGetirAsync()
    {
        return await _context.Araclar
            .AsNoTracking()
            .OrderBy(a => a.Plaka)
            .ToListAsync();
    }

    public async Task<Sonuc> EkleAsync(AracFormViewModel model)
    {
        var plaka = PlakaNormalizeEt(model.Plaka);

        if (await _context.Araclar.AnyAsync(a => a.Plaka == plaka))
            return Sonuc.Hatali($"{plaka} plakalı araç zaten kayıtlı.");

        var arac = new Arac
        {
            Plaka = plaka,
            AracTipi = model.AracTipi!.Value,
            Marka = model.Marka.Trim(),
            Model = model.Model.Trim(),
            OlusturmaTarihi = DateTime.Now
        };

        _context.Araclar.Add(arac);
        await _context.SaveChangesAsync();

        return Sonuc.Basarili();
    }

    private static string PlakaNormalizeEt(string plaka)
    {
        return Regex.Replace(plaka, @"\s+", "").ToUpperInvariant();
    }
}