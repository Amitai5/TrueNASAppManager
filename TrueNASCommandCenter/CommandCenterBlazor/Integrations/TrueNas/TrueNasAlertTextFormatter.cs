using System.Net;
using System.Text;

namespace TrueNasCommandCenter.Integrations.TrueNas;

internal static class TrueNasAlertTextFormatter
{
    internal static string Format(TrueNasAlertDto alert, string fallback)
    {
        ArgumentNullException.ThrowIfNull(alert);

        var hasFormattedText = !string.IsNullOrWhiteSpace(alert.Formatted);
        var preferred = FirstNotBlank(alert.Formatted, alert.Text, fallback);
        var plainText = WebUtility.HtmlDecode(hasFormattedText ? RemoveMarkup(preferred) : preferred);
        return string.IsNullOrWhiteSpace(plainText) ? fallback : plainText;
    }

    internal static string SearchText(TrueNasAlertDto alert, string fallback) => $"{alert.ClassName} {Format(alert, fallback)} {alert.Text} {alert.Source} {alert.Node}";

    private static string FirstNotBlank(params string?[] values) => values.First(value => !string.IsNullOrWhiteSpace(value))!.Trim();

    private static string RemoveMarkup(string value)
    {
        var builder = new StringBuilder(value.Length);
        var insideTag = false;
        foreach (var character in value)
        {
            if (character == '<')
            {
                insideTag = true;
                continue;
            }

            if (character == '>' && insideTag)
            {
                insideTag = false;
                builder.Append(' ');
                continue;
            }

            if (!insideTag)
            {
                builder.Append(character);
            }
        }

        return string.Join(' ', builder.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
