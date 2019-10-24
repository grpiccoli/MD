using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConsultaMD.Extensions
{
    public static class Masking
    {
        public static string MaskEmail(this string email)
        {
            var emailsplit = email?.Split('@');
            var domainsplit = emailsplit[1].Split('.');
            IEnumerable<char> user = emailsplit[0].ToCharArray().ToList();
            IEnumerable<char> provider = domainsplit[0].ToCharArray().ToList();
            return $"{Mask(user)}@{Mask(provider)}.{domainsplit[1]}";
        }

        public static string Mask(IEnumerable<char> chars)
        {
            return $"{chars.Take(1).First().ToString(CultureInfo.InvariantCulture)}{new string('*', chars.Count() - 2)}{chars.Reverse().Take(1).First().ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
