// Wms.Domain/Services/IAuthService.cs

using Wms.Domain.Entities;

namespace Wms.Domain.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}





