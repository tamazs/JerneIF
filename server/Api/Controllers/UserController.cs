using Api.DTOs;
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
        return await _userService.GetUsers();
    }
}