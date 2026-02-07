namespace Unseal.Models.Auth;

public record RegisterModel(
    string Email,
    string Username,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string? Address,
    string? PhoneNumber 
    );