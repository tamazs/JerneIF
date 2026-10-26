using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers(SieveModel sieveModel);
    Task<UserDto> GetUserById(string userId, string? callerId, bool callerIsAdmin);
    Task<UserDto> UpdateUser(UpdateUserRequestDto dto, string? callerId, bool callerIsAdmin);
    Task<UserDto> DeleteUser(string userId);
}