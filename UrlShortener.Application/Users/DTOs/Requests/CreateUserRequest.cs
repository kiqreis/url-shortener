using System.ComponentModel.DataAnnotations;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Users.DTOs.Requests;

public class CreateUserRequest
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(180, ErrorMessage = "Email length is too long")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Invalid password")]
    [DataType(DataType.Password)]
    [StringLength(60, ErrorMessage = "Password length is too long", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public static implicit operator User(CreateUserRequest request) => User.Create(request.Email, request.Password);
}