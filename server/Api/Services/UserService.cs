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

    public async Task<UserDto> UpdateUser(UpdateUserRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);

        if (user == null) throw new KeyNotFoundException("User not found.");
        
        var verification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword!);
        if (verification ==  PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("The current passwords do not match.");

        user.FullName = dto.FullName!;
        user.PhoneNumber = dto.PhoneNumber!;
        user.Email = dto.Email!;
        user.UpdatedAt = DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.NewPassword!);
        }
        
        await dbContext.SaveChangesAsync();
        return new UserDto(user);
    }

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