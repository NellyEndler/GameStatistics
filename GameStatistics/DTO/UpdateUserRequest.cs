namespace GameStatistics.DTO
{
    public class UpdateUserRequest
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
