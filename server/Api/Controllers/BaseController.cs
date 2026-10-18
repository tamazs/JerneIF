using System.Security.Claims;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected string? CurrentUserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    protected bool IsAdmin =>
        User.IsInRole(UserRole.Admin.ToString());
}