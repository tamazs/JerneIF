using System.ComponentModel.DataAnnotations;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Api.Services;

public class UserService (JerneDbContext dbContext, ISieveProcessor sieveProcessor) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers(SieveModel sieveModel)
    {
        IQueryable<User> users = dbContext.Users;
        
        users = sieveProcessor.Apply(sieveModel, users);
        
        return await users.Select(u => new UserDto(u)).ToListAsync();
    }

    public async Task<UserDto> GetUserById(string userId, string? callerId, bool callerIsAdmin)
    {
        if (!callerIsAdmin && userId != callerId)
            throw new ForbiddenException("You can only view your own account.");

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) throw new KeyNotFoundException("User not found.");
        return new UserDto(user);
    }

    public async Task<UserDto> UpdateUser(UpdateUserRequestDto dto, string? callerId, bool callerIsAdmin)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        // Ownership: players may only edit themselves. Admins may edit anyone.
        if (!callerIsAdmin && dto.UserId != callerId)
            throw new ForbiddenException("You can only update your own account.");

        // Role is admin-only. Reject unknown values before touching the database.
        var roleChangeRequested = callerIsAdmin && !string.IsNullOrWhiteSpace(dto.Role);
        if (roleChangeRequested && !IsValidRole(dto.Role!))
            throw new ValidationException($"Invalid role. Allowed values: {string.Join(", ", Enum.GetNames<UserRole>())}.");

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);

        if (user == null) throw new KeyNotFoundException("User not found.");
        
        var isPasswordChangeRequested = !string.IsNullOrWhiteSpace(dto.NewPassword);

        if (isPasswordChangeRequested)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword)) throw new ValidationException("Current password is required.");
            
            var verification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword!);
            
            if (verification == PasswordVerificationResult.Failed) throw new ValidationException("The current passwords do not match.");
            
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.NewPassword!);
        }
        
        user.FullName = dto.FullName!;
        user.PhoneNumber = dto.PhoneNumber!;
        user.Email = dto.Email!;
        user.UpdatedAt = DateTime.UtcNow;

        // Role and IsActive are privileged fields: only admins may change them.
        // From a non-admin they are silently ignored.
        if (roleChangeRequested) user.Role = dto.Role!;

        if (callerIsAdmin && dto.IsActive is true)
        {
            user.IsActive = true;
            user.DeletedAt = null;
        }
        
        await dbContext.SaveChangesAsync();
        return new UserDto(user);
    }

    // Exact, case-sensitive match against the enum names, because the JWT role claim is compared as-is.
    private static bool IsValidRole(string role) => Enum.GetNames<UserRole>().Contains(role);

    public async Task<UserDto> DeleteUser(string userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user == null) throw new KeyNotFoundException("User not found.");
        
        user.IsActive = false;
        user.DeletedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
        return new UserDto(user);
    }
}