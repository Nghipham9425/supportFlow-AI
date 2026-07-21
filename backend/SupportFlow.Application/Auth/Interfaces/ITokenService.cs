
using SupportFlow.Domain.Entities;

namespace SupportFlow.Application.Auth.Interfaces;

public interface ITokenService
{
    (string AccessToken, DateTime ExpiresAt) CreateToken(User user);
}