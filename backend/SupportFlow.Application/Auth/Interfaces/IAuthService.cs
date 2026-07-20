using SupportFlow.Application.Auth.DTOs;

namespace SupportFlow.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthUserDto> RegisterAsync(RegisterUserDto request);
}