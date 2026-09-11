namespace Vizus.Application.Auth;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string FullName, DateTime ExpiresAt);