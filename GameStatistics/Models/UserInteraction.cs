namespace GameStatistics.Models
{
    public class UserInteraction
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int InteractionPointId { get; set; }
        public InteractionPoint InteractionPoint { get; set; } = null!;
        public double InteractionTimeInSeconds { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
