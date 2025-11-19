using Api.DTOs;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService (JerneDbContext dbContext) : IUserService
{
    public async Task<List<UserDto>> GetUsers()
    {
        return await dbContext.Users.Select(u => new UserDto(u)).ToListAsync();
    }
}