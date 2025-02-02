﻿using GameStatistics.DTO;
using GameStatistics.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameStatistics.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult?> RegisterUser(UserRequest dto);
        Task<IdentityResult?> RegisterAdmin(UserRequest dto);
        Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUser(LoginRequest request);
        Task<(string jwtToken, string refreshToken)> GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        Task StoreRefreshToken(ApplicationUser user, string refreshToken);
        Task<bool> ValidateRefreshToken(ApplicationUser user, string refreshToken);
        Task<ApplicationUser?> GetUserByUsername(string username);
        Task<ApplicationUser?> GetUserById(string id);
        Task<List<UserResponse>?> GetAllUsers(string? role);
        //public Task <List<ApplicationUser>?> GetAllAdmins();
        Task<UpdateAdminRequest?> UpdateAdmin(UpdateAdminRequest dto, string? id);
        Task<IdentityResult?> UpdateUser(UpdateUserRequest dto, string? id);
        Task<IdentityResult?> DeleteUser(string? id);
        Task<IdentityResult?> DeleteUserAdmin(int id);
        Task<bool> DeleteService(string? currentUserRole, string? currentUserId, int id);
        Task<string> GetUserId(ApplicationUser user);
        Task<bool> SignOut(string userId);
    }
}
