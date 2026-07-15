using System.Globalization;
using System.Text;

namespace Hoaii.Web.Services;

/// <summary>
/// Vietnamese-aware slugifier, factored out of SearchController so the admin CRUD screens can
/// generate slugs the same way the storefront reads them.
/// </summary>
public static class Slug
{
    public static string From(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);

        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }
            sb.Append(char.IsLetterOrDigit(c) ? c : '-');
        }

        return string.Join('-', sb.ToString().Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}
