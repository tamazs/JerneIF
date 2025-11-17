using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class AuthService(JerneDbContext dbContext, IConfiguration configuration) : IAuthService
{
    public async Task<UserDto> RegisterUser(RegisterRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null) throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Role = UserRole.Player,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password);
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return new UserDto(user);
    }

    public async Task<LoginUserDto> LoginUser(LoginRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid username or password");
        
        var verification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verification ==  PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid username or password");
        
        var token = CreateToken(user);

        return new LoginUserDto
        {
            Token = token,
            User = new UserDto(user)
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppOptions:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppOptions:Issuer"),
            audience: configuration.GetValue<string>("AppOptions:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}