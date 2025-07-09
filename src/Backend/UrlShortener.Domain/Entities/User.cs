using System.Text.Json.Serialization;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsVerified { get; private set; }
    public UserPlan Plan { get; private set; }
    public IReadOnlyCollection<ShortUrl> Urls => _urls.AsReadOnly();

    private readonly List<ShortUrl> _urls = [];

    public User(string email)
    {
        Email = email;
    }
    
    [JsonConstructor]
    private User(Guid id, string email)
    {
        Id = id;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        IsVerified = false;
        CreatedAt = DateTime.UtcNow;
        Plan = UserPlan.Free;
    }
    
    public static User Create(Guid id, string email) => new(id, email);

    public void UpgradePlan(UserPlan newUserPlan) => Plan = newUserPlan;

    public void Verify() => IsVerified = true;
    
    public void AddUrl(ShortUrl url)
    {
        if (Plan == UserPlan.Free && _urls.Count >= 30)
        {
            throw new InvalidOperationException("Free plan url limit reached");
        }

        _urls.Add(url);
    }
}