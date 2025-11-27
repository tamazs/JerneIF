using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace Api.Controllers;

[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost(nameof(GetUsers))]
    public async Task<List<UserDto>> GetUsers([FromBody] SieveModel sieveModel)
    {
        return await _userService.GetAllUsers(sieveModel);
    }

    [Authorize]
    [HttpPut(nameof(UpdateUser))]
    public async Task<UserDto> UpdateUser([FromBody] UpdateUserRequestDto dto)
    {
        return await _userService.UpdateUser(dto);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut(nameof(DeleteUser))]
    public async Task<UserDto> DeleteUser()
    {
        return await _userService.DeleteUser(CurrentUserId);
    }
}