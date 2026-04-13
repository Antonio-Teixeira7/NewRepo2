using System.Net.Http.Json;
using System.Net.Http.Headers;
using UserSite.Front.Models;

namespace UserSite.Front.Services;

public class UserApiClient(HttpClient httpClient, UserSessionService userSessionService)
{
	public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		using var request = CreateAuthorizedRequest(HttpMethod.Get, "api/user");
		using var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken: cancellationToken) ?? [];
	}

	public async Task<UserDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		using var request = CreateAuthorizedRequest(HttpMethod.Get, $"api/user/by-name?name={Uri.EscapeDataString(name)}");
		using var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();

		var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken);
		return user ?? throw new InvalidOperationException("API returned an empty response.");
	}

	public async Task CreateAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsJsonAsync("api/user", user, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateAsync(int id, UpdateUserRequest user, CancellationToken cancellationToken = default)
	{
		using var request = CreateAuthorizedRequest(HttpMethod.Put, $"api/user?id={id}");
		request.Content = JsonContent.Create(user);
		var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
	{
		using var request = CreateAuthorizedRequest(HttpMethod.Put, $"api/user/activate?id={id}");
		var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
	{
		using var request = CreateAuthorizedRequest(HttpMethod.Put, $"api/user/deactivate?id={id}");
		var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string uri)
	{
		var request = new HttpRequestMessage(method, uri);

		if (userSessionService.IsAuthenticated)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userSessionService.Token);
		}

		return request;
	}
}
