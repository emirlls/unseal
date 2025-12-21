namespace Unseal.Dtos.Auth;

public record RegisterDto(
    string Email, 
    string Password, 
    string ConfirmPassword, 
    string FirstName, 
    string LastName, 
    string? Address, 
    string? PhoneNumber
    );