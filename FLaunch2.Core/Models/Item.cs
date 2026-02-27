using LiteDB;

namespace FLaunch2.Models
{
    public class Item
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DisplayName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string WorkingDirectory { get; set; } = string.Empty;
        public double Score { get; set; } = 0.0;
        public DateTimeOffset LastExecuted { get; set; }
        public string Arguments { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string[] Tags { get; set; } = [];

        public static double CalculateInitialScore(ICollection<Item> items, double initialScoreRate)
        {
            if (items.Count == 0)
            {
                return 1.0;
            }
            var maxScore = items.Max(x => x.Score);
            return Math.Max(maxScore * initialScoreRate, double.Epsilon);
        }
    }
}
