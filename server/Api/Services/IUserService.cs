using Api.DTOs;

namespace Api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsers();
}