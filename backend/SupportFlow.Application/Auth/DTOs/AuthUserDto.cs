using SupportFlow.Domain.Enums;

namespace SupportFlow.Application.Auth.DTOs;

public class AuthUserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
}