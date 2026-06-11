using LiteDB;

namespace FLaunch2.Models
{
    public class Item
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DisplayName { get => field; set => field = value ?? string.Empty; } = string.Empty;
        public string FilePath { get => field; set => field = value ?? string.Empty; } = string.Empty;
        public string WorkingDirectory { get => field; set => field = value ?? string.Empty; } = string.Empty;
        public double Score { get; set; } = 0.0;
        public DateTimeOffset LastExecuted { get; set; }
        public string Arguments { get => field; set => field = value ?? string.Empty; } = string.Empty;
        public string Comment { get => field; set => field = value ?? string.Empty; } = string.Empty;
        public string[] Tags { get; set; } = [];

        public bool IncreaseScore(ICollection<Item> items, double scoreIncreaseRate)
        {
            var maxScore = items.Max(x => x.Score);
            double delta = Math.Max(maxScore * scoreIncreaseRate, double.Epsilon);
            if (Score > double.MaxValue - delta)
            {
                var i = 0;
                foreach (var item in items.OrderBy(x => x.Score).ToArray())
                {
                    i++;
                    item.Score /= maxScore;
                    if (item.Score <= 0)
                    {
                        item.Score = i * double.Epsilon;
                    }
                }
                Score += scoreIncreaseRate;
                return true;
            }
            else
            {
                Score += delta;
                return false;
            }
        }

        public bool Equals(Item other, ItemEquivalenceCondition condition)
        {
            if (other == null)
            {
                return false;
            }
            if (condition.DisplayName && !string.Equals(DisplayName, other.DisplayName, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            if (condition.FilePath && !string.Equals(FilePath, other.FilePath, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            if (condition.WorkingDirectory && !string.Equals(WorkingDirectory, other.WorkingDirectory, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            if (condition.Arguments && !string.Equals(Arguments, other.Arguments, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static double CalculateInitialScore(ICollection<Item> items, double initialScoreRate)
        {
            if (items.Count == 0)
            {
                return 1.0;
            }
            var maxScore = items.Max(x => x.Score);
            return Math.Max(maxScore * initialScoreRate, double.Epsilon);
        }

        public static IEnumerable<string> GetAllTags(IEnumerable<Item> items)
        {
            return items.SelectMany(x => x.Tags).Distinct();
        }
    }
}