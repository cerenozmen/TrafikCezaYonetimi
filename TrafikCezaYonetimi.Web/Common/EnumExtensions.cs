using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TrafikCezaYonetimi.Web.Common;

public static class EnumExtensions
{
    public static string GorunenAd(this Enum deger)
    {
        var uye = deger.GetType().GetMember(deger.ToString()).FirstOrDefault();
        var display = uye?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? deger.ToString();
    }
}