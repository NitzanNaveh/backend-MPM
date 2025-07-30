using ProjectManager.Api.Dtos.Auth;

namespace ProjectManager.Api.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
} 