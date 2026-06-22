using FLaunch2.Models;
using System.Globalization;

namespace FLaunch2.Services;

public static class FLaunch1Writer
{
    public static void WriteItems(string filePath, IEnumerable<Item> items)
    {
        using TextWriter writer = new StreamWriter(filePath);
        WriteItems(writer, items);
    }

    public static void WriteItems(TextWriter writer, IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            writer.WriteLine(FormatLine(item));
        }
    }

    private static string FormatLine(Item item)
    {
        var fields = new[]
        {
            Normalize(item.DisplayName),
            Normalize(item.FilePath),
            Normalize(item.WorkingDirectory),
            item.Score.ToString("R", CultureInfo.InvariantCulture),
            item.LastExecuted.ToString("o", CultureInfo.InvariantCulture),
            Normalize(item.Arguments),
            Normalize(item.Comment),
            string.Join(',', item.Tags ?? []),
        };

        return string.Join('\t', fields);
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty)
            .Replace('\t', ' ')
            .Replace('\r', ' ')
            .Replace('\n', ' ');
    }
}
