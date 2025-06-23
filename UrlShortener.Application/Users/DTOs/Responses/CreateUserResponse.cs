using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Users.DTOs.Responses;

public class CreateUserResponse(string Email, UserPlan userPlan);