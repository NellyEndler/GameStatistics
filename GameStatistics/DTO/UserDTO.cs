﻿namespace GameStatistics.DTO
{
    public class UserDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? Role { get; set; }
    }
}
