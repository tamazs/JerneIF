using Api.DTOs;
using Api.DTOs.Request;

namespace Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> UpdateUser(UpdateUserRequestDto dto);
    Task<UserDto> DeleteUser(string userId);
}