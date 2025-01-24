namespace GameStatistics.DTO
{
    public class UpdateUserDTO
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
