using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ConsultaMD.Extensions
{
    public static partial class EnumUtils
    {
        public static MultiSelectList Enum2MultiSelect<TEnum>(
            IDictionary<string, List<string>> Filters = null,
            string name = null)
            where TEnum : struct, IConvertible, IFormattable
        {
            var tipo = typeof(TEnum);
            var nombre = tipo.ToString().Split('.').Last();
            return name switch
            {
                "Description" =>
                    new MultiSelectList(
                        from TEnum s in Enum.GetValues(tipo)
                        select new
                        { Value = s.ToString("d", null), Text = s.GetAttrDescription() },
                        Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null),
                "Name" =>
                    new MultiSelectList(
                        from TEnum s in Enum.GetValues(tipo)
                        select new
                        { Value = s.ToString("d", null), Text = s.GetAttrName() },
                        Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null),
                _ =>
                    new MultiSelectList(
                        from TEnum s in Enum.GetValues(tipo)
                        select new
                        { Value = s.ToString("d", null), Text = s.ToString() },
                        Filters != null && Filters.ContainsKey(nombre) ? Filters[nombre] : null)
            };
        }

        public static IEnumerable<SelectListItem> Enum2Select<TEnum>(string name = null)
            where TEnum : struct, IConvertible, IFormattable
        {
            return name switch
            {
                "Name" =>
                    ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.GetAttrName()
                        }).ToList(),
                "Description" =>
                    ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.GetAttrDescription()
                        }).ToList(),
                _ =>
                    ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new SelectListItem
                        {
                            Value = t.ToString("d", null),
                            Text = t.ToString()
                        }).ToList()
            };
        }

        public static IEnumerable<MsSelect> Enum2Ms<TEnum>(string name = null)
            where TEnum : struct, IConvertible, IFormattable
        {
            return name switch
            {
                "Name" => ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new MsSelect
                        {
                            Value = Convert.ToInt32(t, CultureInfo.InvariantCulture),
                            Text = t.GetAttrName()
                        }).ToList(),
                "Description" => ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new MsSelect
                        {
                            Value = Convert.ToInt32(t, CultureInfo.InvariantCulture),
                            Text = t.GetAttrDescription()
                        }).ToList(),
                _ => ((TEnum[])Enum.GetValues(typeof(TEnum)))
                        .Select(t => new MsSelect
                        {
                            Value = Convert.ToInt32(t, CultureInfo.InvariantCulture),
                            Text = t.ToString()
                        }).ToList()
            };
        }

        public static MultiSelectList Enum2MultiSelect<TEnum>(string name = null)
            where TEnum : struct, IConvertible, IFormattable
        {
            var tipo = typeof(TEnum);
            var nombre = tipo.ToString().Split('.').Last();
            return name switch
            {
                "Description" => new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Value = s.ToString("d", null), Text = s.GetAttrDescription() }),
                "Name" => new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Value = s.ToString("d", null), Text = s.GetAttrName() }),
                _ => new MultiSelectList(
                            from TEnum s in Enum.GetValues(tipo)
                            select new
                            { Value = s.ToString("d", null), Text = s.ToString() })
            };
        }

        public static string GetAttrDescription<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.Description
              ?? e.ToString();
        }

        public static string GetAttrPrompt<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.Prompt
              ?? e.ToString();
        }
        public static string GetAttrName<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>(false)
                ?.Name
                ?? e.ToString();
        }
        //public static string GetAttrName<TEnum>(this TEnum e, string lang)
        //{
        //    switch (lang)
        //    {
        //        default:
        //        case "es":
        //            var rm = new ResourceManager(typeof(EnumResources));
        //            var name = e.GetType().Name + "_" + e;
        //            var resourceDisplayName = rm.GetString(name);
        //            return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", e) : resourceDisplayName;
        //    }
        //}
        public static string GetAttrGroupName<TEnum>(this TEnum e)
        {
            return e.GetType()
                .GetMember(e.ToString())
              .FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>(false)
              ?.GroupName
              ?? e.ToString();
        }
    }
    public class MsSelect
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
