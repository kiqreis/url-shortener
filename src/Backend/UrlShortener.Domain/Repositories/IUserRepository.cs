﻿using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
}