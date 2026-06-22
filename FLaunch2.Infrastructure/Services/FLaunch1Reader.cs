using FLaunch2.Models;
using System.Globalization;

namespace FLaunch2.Services
{
    public class FLaunch1Reader
    {
        public static IEnumerable<Item> ReadItems(string filePath)
        {
            using TextReader sr = new StreamReader(filePath);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return ParseLine(line);
            }
        }

        private static Item ParseLine(string line)
        {
            var item = line.Split('\t');
            return new Item()
            {
                DisplayName = item[0],
                FilePath = item[1],
                WorkingDirectory = item[2],
                Score = ParseScore(item[3]),
                LastExecuted = ParseLastExecuted(item[4]),
                Arguments = item.Length > 5 ? item[5] : "",
                Comment = item.Length > 6 ? item[6] : "",
                Tags = item.Length > 7 ? item[7].Split(',') : [],
            };
        }

        private static double ParseScore(string value)
        {
            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var invariantScore))
            {
                return invariantScore;
            }

            if (double.TryParse(value, out var score))
            {
                return score;
            }

            return 1.0;
        }

        private static DateTimeOffset ParseLastExecuted(string value)
        {
            if (DateTimeOffset.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var invariantDateTime))
            {
                return invariantDateTime;
            }

            if (DateTimeOffset.TryParse(value, out var lastExecuted))
            {
                return lastExecuted;
            }

            return DateTimeOffset.MinValue;
        }
    }
}
