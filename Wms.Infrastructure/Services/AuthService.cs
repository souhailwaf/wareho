// Wms.Infrastructure/Services/AuthService.cs

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Wms.Domain.Entities;
using Wms.Domain.Repositories;
using Wms.Domain.Services;

namespace Wms.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await _userRepository.GetByUsernameOrEmailAsync(usernameOrEmail, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("Authentication failed: User not found - {UsernameOrEmail}", usernameOrEmail);
            return null;
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning("Authentication failed: Invalid password for user - {UsernameOrEmail}", usernameOrEmail);
            return null;
        }

        _logger.LogInformation("User authenticated successfully: {Username}", user.Username);
        return user;
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        var hashedPassword = HashPassword(password);
        return hashedPassword == passwordHash;
    }
}





