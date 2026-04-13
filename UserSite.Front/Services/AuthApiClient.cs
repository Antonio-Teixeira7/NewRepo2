using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using UserSite.Front.Configuration;
using UserSite.Front.Models;

namespace UserSite.Front.Services;

public class AuthApiClient(HttpClient httpClient, IOptions<AuthApiSettings> authApiSettings)
{
	private readonly AuthApiSettings _authApiSettings = authApiSettings.Value;

	public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsJsonAsync(_authApiSettings.LoginPath, request, cancellationToken);
		response.EnsureSuccessStatusCode();

		var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
		if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
		{
			throw new InvalidOperationException("O AuthService retornou uma resposta de login inválida.");
		}

		return payload;
	}
}
