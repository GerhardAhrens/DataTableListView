//-----------------------------------------------------------------------
// <copyright file="Humanizer.cs" company="Lifeprojects.de">
//     Class: Humanizer
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>08.08.2025 13:38:19</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace DataTableListView.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class Humanizer
    {
        public static string Get(string msg, int args)
        {
            string result = string.Empty;

            List<string> argsSource = msg.ExtractFromString("[", "]").ToList();

            if (argsSource != null && argsSource.Count > 0)
            {
                foreach (string item in argsSource)
                {
                    string changeText = string.Empty;
                    if (args == 1)
                    {
                        changeText = item.Split('/')[0];
                    }
                    else
                    {
                        changeText = item.Split('/')[1];
                    }

                    msg = msg.Replace(item, changeText);
                }

                if (args > 1)
                {
                    result = string.Format(CultureInfo.CurrentCulture, msg.Replace("[", string.Empty).Replace("]", string.Empty), args);
                }
                else
                {
                    if (args == 0)
                    {
                        if (argsSource.First().Length > 2)
                        {
                            string valueNull = argsSource.First().Split('/').Last();
                            result = msg.Replace("[", string.Empty).Replace("]", string.Empty).Replace("{0}", valueNull);
                        }
                        else
                        {
                            result = msg.Replace("[", string.Empty).Replace("]", string.Empty);
                        }
                    }
                    else
                    {
                        result = msg.Replace("[", string.Empty).Replace("]", string.Empty);
                    }
                }
            }

            return result;
        }
    }

    public static class StringExtractExtensions
    {
        public static IEnumerable<string> ExtractFromString(this string @this, string startString, string endString)
        {
            if (@this == null || startString == null || endString == null)
            {
                yield return null;
            }

            Regex r = new Regex(Regex.Escape(startString) + "(.*?)" + Regex.Escape(endString));
            MatchCollection matches = r.Matches(@this);
            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value;
            }
        }

        public static T[] ExtractContent<T>(this string @this, string regex)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
            if (tc.CanConvertFrom(typeof(string)) == false)
            {
                throw new ArgumentException("Type does not have a TypeConverter from string", "T");
            }
            if (string.IsNullOrEmpty(@this) == false)
            {
                return
                    Regex.Matches(@this, regex)
                    .Cast<Match>()
                    .Select(f => f.ToString())
                    .Select(f => (T)tc.ConvertFrom(f))
                    .ToArray();
            }
            else
            {
                return [];
            }
        }

        public static int[] ExtractInts(this string @this)
        {
            return @this.ExtractContent<int>(@"\d+");
        }
    }
}
