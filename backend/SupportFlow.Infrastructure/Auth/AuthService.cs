using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Auth.DTOs;
using SupportFlow.Application.Auth.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;
using SupportFlow.Infrastructure.Persistence;


namespace SupportFlow.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
private readonly ITokenService _tokenService;
    public AuthService(
        AppDbContext db,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthUserDto> RegisterAsync(RegisterUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException(
                "Name, email, and password are required.");
        }

        if (request.Password.Length < 8)
        {
            throw new InvalidOperationException(
                "Password must be at least 8 characters long.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _db.Users
            .AnyAsync(user => user.Email == email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = UserRole.Customer
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ToDto(user);
    }

    private static AuthUserDto ToDto(User user)
    {
        return new AuthUserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);

        if (user is null)
        {
            throw new InvalidOperationException(
            "Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        request.Password);
        
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Invalid email or password.");
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

            await _db.SaveChangesAsync();
        }
        var (accessToken, expiresAt) = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            User = ToDto(user)
        };
    }
}