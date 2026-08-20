using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportFlow.Domain.Entities;
using SupportFlow.Domain.Enums;

namespace SupportFlow.Infrastructure.Persistence;

public static class DemoAdminSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHasher<User> passwordHasher,
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Demo admin name, email, and password are required when seeding is enabled.");
        }

        if (password.Length < 8)
        {
            throw new InvalidOperationException(
                "Demo admin password must be at least 8 characters long.");
        }

        var normalizedName = name.Trim();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

        if (user is null)
        {
            user = new User
            {
                Name = normalizedName,
                Email = normalizedEmail,
                Role = UserRole.Admin
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);
            return;
        }

        var hasChanges = false;

        if (user.Name != normalizedName)
        {
            user.Name = normalizedName;
            hasChanges = true;
        }

        if (user.Role != UserRole.Admin)
        {
            user.Role = UserRole.Admin;
            hasChanges = true;
        }

        var passwordResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (passwordResult != PasswordVerificationResult.Success)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
