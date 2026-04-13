namespace UserSite.Front.Services;

public class UserSessionService
{
	public string Token { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;
	public DateTime? ExpiresAt { get; private set; }

	public bool IsAuthenticated =>
		!string.IsNullOrWhiteSpace(Token) &&
		(!ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow);

	public void SetSession(string token, string email, DateTime? expiresAt)
	{
		Token = token;
		Email = email;
		ExpiresAt = expiresAt;
	}

	public void Clear()
	{
		Token = string.Empty;
		Email = string.Empty;
		ExpiresAt = null;
	}
}
