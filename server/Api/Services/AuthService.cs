using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.DTOs;
using Api.DTOs.Request;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class AuthService(JerneDbContext dbContext, IConfiguration configuration, IMemoryCache cache) : IAuthService
{
    // Login lockout: after MaxFailedLogins wrong attempts for the same email, further attempts are refused
    // until LockoutDuration has passed since the last failure. The counter lives in memory, so it is
    // per server instance and resets on restart. It limits online password guessing, it is not a full defence.
    private const int MaxFailedLogins = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static string LoginFailureKey(string email) => $"login-failures:{email.Trim().ToLowerInvariant()}";

    private void EnsureNotLockedOut(string key)
    {
        if (cache.TryGetValue(key, out int failures) && failures >= MaxFailedLogins)
            throw new UnauthorizedAccessException("Too many failed login attempts. Try again later.");
    }

    private void RegisterLoginFailure(string key)
    {
        cache.TryGetValue(key, out int failures);
        cache.Set(key, failures + 1, LockoutDuration);
    }

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
            Role = UserRole.Player.ToString(),
            IsActive = false,
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
        
        var failureKey = LoginFailureKey(dto.Email);
        EnsureNotLockedOut(failureKey);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            RegisterLoginFailure(failureKey);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        
        var verification = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verification ==  PasswordVerificationResult.Failed)
        {
            RegisterLoginFailure(failureKey);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        cache.Remove(failureKey);
        
        if (!user.IsActive) throw new UnauthorizedAccessException("Your account is not active");
        
        var token = CreateToken(user);

        return new LoginUserDto
        {
            Token = token,
            RefreshToken = await GenerateAndSaveRefreshToken(user),
            User = new UserDto(user)
        };
    }

    public async Task<LoginUserDto> RefreshTokens(RefreshTokenRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

        if (user == null || !user.IsActive || user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token");
        
        var newAccessToken = CreateToken(user);
        var newRefreshToken = await GenerateAndSaveRefreshToken(user);

        return new LoginUserDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new UserDto(user)
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Role, user.Role),
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