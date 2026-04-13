namespace UserSite.Front.Services;

public class UserSessionService
{
	public string Token { get; private set; } = string.Empty;
	public int UserId { get; private set; }
	public string Email { get; private set; } = string.Empty;
	public DateTime? ExpiresAtUtc { get; private set; }

	public bool IsAuthenticated =>
		!string.IsNullOrWhiteSpace(Token) &&
		(!ExpiresAtUtc.HasValue || ExpiresAtUtc.Value > DateTime.UtcNow);

	public void SetSession(string token, int userId, string email, DateTime? expiresAtUtc)
	{
		Token = token;
		UserId = userId;
		Email = email;
		ExpiresAtUtc = expiresAtUtc;
	}

	public void Clear()
	{
		Token = string.Empty;
		UserId = 0;
		Email = string.Empty;
		ExpiresAtUtc = null;
	}
}
