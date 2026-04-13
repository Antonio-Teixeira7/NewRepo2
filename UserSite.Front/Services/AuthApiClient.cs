using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UserSite.Front.Configuration;
using UserSite.Front.Models;

namespace UserSite.Front.Services;

public class AuthApiClient(HttpClient httpClient, IOptions<AuthApiSettings> authApiSettings)
{
	private readonly AuthApiSettings _authApiSettings = authApiSettings.Value;

	public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
	{
		using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _authApiSettings.LoginPath)
		{
			Content = JsonContent.Create(request)
		};

		using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
		response.EnsureSuccessStatusCode();

		var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(new JsonSerializerOptions(JsonSerializerDefaults.Web), cancellationToken);
		if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
		{
			throw new InvalidOperationException("O AuthService retornou uma resposta de login inválida.");
		}

		return payload;
	}
}
