using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities;

public class User
{
    public string Email { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsVerified { get; private set; }
    public UserPlan Plan { get; private set; }
    public IReadOnlyCollection<ShortUrl> Urls => _urls.AsReadOnly();

    private readonly List<ShortUrl> _urls = [];
    
    private User(string email, string password)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        CreatedAt = DateTime.UtcNow;
        Plan = UserPlan.Free;
    }

    public static User Create(string email, string password) => new(email, password);

    public void UpgradePlan(UserPlan newUserPlan) => Plan = newUserPlan;

    public void AddUrl(ShortUrl url)
    {
        if (Plan == UserPlan.Free && _urls.Count >= 30)
        {
            throw new InvalidOperationException("Free plan url limit reached");
        }

        _urls.Add(url);
    }
}