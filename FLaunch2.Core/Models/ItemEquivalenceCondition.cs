namespace FLaunch2.Models
{
    public class ItemEquivalenceCondition
    {
        public bool DisplayName { get; set; } = true;
        public bool FilePath { get; set; } = true;
        public bool WorkingDirectory { get; set; } = true;
        public bool Arguments { get; set; } = true;
        public bool ItemType { get; set; } = true;
    }
}
