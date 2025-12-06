using Api.DTOs;
using Api.DTOs.Request;
using Sieve.Models;

namespace Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers(SieveModel sieveModel);
    Task<UserDto> GetUserById(string userId);
    Task<UserDto> UpdateUser(UpdateUserRequestDto dto);
    Task<UserDto> DeleteUser(string userId);
}