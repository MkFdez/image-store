using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum OrderByModel
    {
        [Display(Name ="Downloads")]
        DownloadsDesc,
        [Display(Name ="Price low to high")]
        Price,
        [Display(Name ="Price high to low")]
        PriceDesc,
        [Display(Name = "Stars")]
        StarsDesc,
        [Display(Name ="Newer First")]
        Date,
        [Display(Name = "Older First")]
        DateDesc,
    }
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            return value.GetType()
              .GetMember(value.ToString())
              .First()
              .GetCustomAttribute<DisplayAttribute>()
              ?.GetName();
        }
    }
}
