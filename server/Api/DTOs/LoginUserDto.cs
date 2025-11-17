namespace Api.DTOs;

public class LoginUserDto
{
    public string Token { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}