namespace Security.Api.Application.DTOs.Authentication;

public sealed record LoginRequest(string Email, string Password);