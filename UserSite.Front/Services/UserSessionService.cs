using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace UserSite.Front.Services;

public class UserSessionService(ProtectedLocalStorage protectedLocalStorage)
{
	private const string TokenKey = "auth.token";
	private const string UserIdKey = "auth.userId";
	private const string EmailKey = "auth.email";
	private const string ExpiresAtUtcKey = "auth.expiresAtUtc";

	public string Token { get; private set; } = string.Empty;
	public int UserId { get; private set; }
	public string Email { get; private set; } = string.Empty;
	public DateTime? ExpiresAtUtc { get; private set; }

	public bool IsAuthenticated =>
		!string.IsNullOrWhiteSpace(Token) &&
		(!ExpiresAtUtc.HasValue || ExpiresAtUtc.Value > DateTime.UtcNow);

	public async Task RestoreAsync()
	{
		var tokenResult = await protectedLocalStorage.GetAsync<string>(TokenKey);
		var userIdResult = await protectedLocalStorage.GetAsync<int>(UserIdKey);
		var emailResult = await protectedLocalStorage.GetAsync<string>(EmailKey);
		var expiresAtUtcResult = await protectedLocalStorage.GetAsync<DateTime?>(ExpiresAtUtcKey);

		Token = tokenResult.Success ? tokenResult.Value ?? string.Empty : string.Empty;
		UserId = userIdResult.Success ? userIdResult.Value : 0;
		Email = emailResult.Success ? emailResult.Value ?? string.Empty : string.Empty;
		ExpiresAtUtc = expiresAtUtcResult.Success ? expiresAtUtcResult.Value : null;
	}

	public async Task SetSessionAsync(string token, int userId, string email, DateTime? expiresAtUtc)
	{
		Token = token;
		UserId = userId;
		Email = email;
		ExpiresAtUtc = expiresAtUtc;

		await protectedLocalStorage.SetAsync(TokenKey, token);
		await protectedLocalStorage.SetAsync(UserIdKey, userId);
		await protectedLocalStorage.SetAsync(EmailKey, email);
		await protectedLocalStorage.SetAsync(ExpiresAtUtcKey, expiresAtUtc);
	}

	public async Task ClearAsync()
	{
		Token = string.Empty;
		UserId = 0;
		Email = string.Empty;
		ExpiresAtUtc = null;

		await protectedLocalStorage.DeleteAsync(TokenKey);
		await protectedLocalStorage.DeleteAsync(UserIdKey);
		await protectedLocalStorage.DeleteAsync(EmailKey);
		await protectedLocalStorage.DeleteAsync(ExpiresAtUtcKey);
	}
}
