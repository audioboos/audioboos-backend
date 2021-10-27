using System;
using System.Globalization;

namespace AudioBoos.Server.Helpers;

public static class StringExtensions {
    public static string ToTitleCase(this string source) =>
        new CultureInfo(CultureInfo.CurrentCulture.Name, false).TextInfo.ToTitleCase(source);

    public static string TrimTheFromStart(this string str) =>
        str.TrimStart("the", comparisonType: StringComparison.InvariantCultureIgnoreCase);

    public static string TrimStr(this string str, string trimStr,
        bool trimEnd = true, bool repeatTrim = true,
        StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) {
        int strLen;
        do {
            strLen = str.Length;
            {
                if (trimEnd) {
                    if (!(str ?? "").EndsWith(trimStr)) return str;
                    var pos = str.LastIndexOf(trimStr, comparisonType);
                    if ((!(pos >= 0)) || (str.Length - trimStr.Length != pos)) break;
                    str = str.Substring(0, pos);
                } else {
                    if (!(str ?? "").StartsWith(trimStr)) return str;
                    var pos = str.IndexOf(trimStr, comparisonType);
                    if (pos != 0) break;
                    str = str.Substring(trimStr.Length, str.Length - trimStr.Length);
                }
            }
        } while (repeatTrim && strLen > str.Length);

        return str;
    }

    public static string TrimEnd(this string str, string trimStr,
        bool repeatTrim = true,
        StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        => TrimStr(str, trimStr, true, repeatTrim, comparisonType);

    public static string TrimStart(this string str, string trimStr,
        bool repeatTrim = true,
        StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        => TrimStr(str, trimStr, false, repeatTrim, comparisonType);

    public static string Trim(this string str, string trimStr, bool repeatTrim = true,
        StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        => str.TrimStart(trimStr, repeatTrim, comparisonType)
            .TrimEnd(trimStr, repeatTrim, comparisonType);
}
