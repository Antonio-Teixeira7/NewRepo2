namespace UserSite.Configurations;

public class AuthIntegrationOptions
{
	public const string SectionName = "AuthIntegration";

	public string InternalApiKey { get; set; } = string.Empty;
}
