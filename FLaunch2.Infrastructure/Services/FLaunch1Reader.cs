using FLaunch2.Models;

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
                Score = double.TryParse(item[3], out var score) ? score : 1.0,
                LastExecuted = DateTimeOffset.TryParse(item[4], out var lastExecuted) ? lastExecuted : DateTimeOffset.MinValue,
                Arguments = item.Length > 5 ? item[5] : "",
                Comment = item.Length > 6 ? item[6] : "",
                Tags = item.Length > 7 ? item[7].Split(',') : [],
            };
        }
    }
}
