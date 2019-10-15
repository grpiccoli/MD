using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsultaMD.Extensions
{
    public class Enum<TEnum> where TEnum: struct, IConvertible, IFormattable
    {
        public static IEnumerable<SelectListItem> ToSelect
        {
            get
            {
                return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString("d", null),
                        Text = ((DisplayAttribute[])t.GetType().GetField(t.ToString())
                        .GetCustomAttributes(typeof(DisplayAttribute), false))[0].Name
                    }).ToList();
            }
        }
    }
}
