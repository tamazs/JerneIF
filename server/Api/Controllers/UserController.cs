using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(nameof(GetUsers))]
    public async Task<List<UserDto>> GetUsers()
    {
        return await _userService.GetAllUsers();
    }

    [HttpPut(nameof(UpdateUser))]
    public async Task<UserDto> UpdateUser([FromBody] UpdateUserRequestDto dto)
    {
        return await _userService.UpdateUser(dto);
    }
    
    [HttpPut(nameof(DeleteUser))]
    public async Task<UserDto> DeleteUser(string userId)
    {
        return await _userService.DeleteUser(userId);
    }
}