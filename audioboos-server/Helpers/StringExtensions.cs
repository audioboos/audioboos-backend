using System.Globalization;

namespace AudioBoos.Server.Helpers;

public static class StringExtensions {
    public static string ToTitleCase(this string source) =>
        new CultureInfo(CultureInfo.CurrentCulture.Name, false).TextInfo.ToTitleCase(source);
}
