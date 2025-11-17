using Api.DTOs;
using Api.DTOs.Request;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(nameof(RegisterUser))]
    public async Task<UserDto> RegisterUser([FromBody] RegisterRequestDto dto)
    {
        return await _authService.RegisterUser(dto);
    }

    [HttpPost(nameof(LoginUser))]
    public async Task<LoginUserDto> LoginUser([FromBody] LoginRequestDto dto)
    {
        return await _authService.LoginUser(dto);
    }

    [Authorize]
    [HttpGet(nameof(AuthenticatedEndpoint))]
    public IActionResult AuthenticatedEndpoint()
    {
        return Ok("You have been authenticated");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet(nameof(AdminAuthenticatedEndpoint))]
    public IActionResult AdminAuthenticatedEndpoint()
    {
        return Ok("You have been authenticated as admin");
    }
}