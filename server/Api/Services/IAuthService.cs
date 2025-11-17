using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;

namespace Api.Services;

public interface IAuthService
{
    Task<UserDto> RegisterUser(RegisterRequestDto dto);
    Task<LoginUserDto> LoginUser(LoginRequestDto dto);
}