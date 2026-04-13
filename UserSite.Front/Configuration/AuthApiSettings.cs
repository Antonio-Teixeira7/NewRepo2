namespace UserSite.Front.Configuration;

public class AuthApiSettings
{
	public const string SectionName = "AuthApiSettings";

	public string BaseUrl { get; set; } = "https://usersite-auth-service.onrender.com/";
	public string LoginPath { get; set; } = "/api/Auth/login";
}
