using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ControllerExample : ControllerBase
{
    private readonly IServiceExample _exampleService;

    public ControllerExample(IServiceExample serviceExample)
    {
        _exampleService = serviceExample;
    }
}