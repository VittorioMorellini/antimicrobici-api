using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Utils
{
    public static class StringUtils
    {
        public static string TrySubstring(this string value, int startIndex, int length)
        {
            string result = value;
            if (!string.IsNullOrEmpty(result) && result.Length > (startIndex + length))
            {
                result = result.Substring(startIndex, length);
            }

            return result;
        }

        public static string CleanString(this string value, params string[] tokens)
        {
            string result = value;
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = result.Replace("\"", " ");
                result = result.Replace("'", " ");
                result = result.Replace("/", " ");
                result = result.Replace("  ", " ");

                foreach (var t in tokens)
                    result = result.Replace(t, " ");

                result = result.Trim();
            }

            return result;
        }

        public static string RemoveSpaces(this string value)
        {
            string result = value;
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = result.Replace(" ", "");
            }

            return result?.Trim();
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static string ToPascalCase(this string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }
    }
}

