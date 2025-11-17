using DataAccess;

namespace Api.DTOs;

public class UserDto
{
    public UserDto(User user)
    {
        UserId =  user.UserId;
        FullName = user.FullName;
        PhoneNumber = user.PhoneNumber;
        Email = user.Email;
        Role = user.Role;
        IsActive = user.IsActive;
    }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}