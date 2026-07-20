namespace SupportFlow.Application.Auth.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public AuthUserDto User { get; set; } = new();
}