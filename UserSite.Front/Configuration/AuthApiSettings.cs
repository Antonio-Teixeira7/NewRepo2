namespace UserSite.Front.Configuration;

public class AuthApiSettings
{
	public const string SectionName = "AuthApiSettings";

	public string BaseUrl { get; set; } = string.Empty;
	public string LoginPath { get; set; } = "api/auth/login";
}
