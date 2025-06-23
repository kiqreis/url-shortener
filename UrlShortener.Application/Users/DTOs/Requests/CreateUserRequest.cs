using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Users.DTOs.Requests;

public record CreateUserRequest(string Email, string Password)
{
    public static implicit operator User(CreateUserRequest request) => User.Create(request.Email, request.Password);
}