using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Auth.DTOs;
using SupportFlow.Application.Auth.Interfaces;

namespace SupportFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<AuthUserDto>> Register(RegisterUserDto req)
    {
        try
        {
            var user = await _authService.RegisterAsync(req);
            return StatusCode(StatusCodes.Status201Created, user);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}