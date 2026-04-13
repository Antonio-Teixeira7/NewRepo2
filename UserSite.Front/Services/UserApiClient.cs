using System.Net.Http.Json;
using UserSite.Front.Models;

namespace UserSite.Front.Services;

public class UserApiClient(HttpClient httpClient)
{
	public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await httpClient.GetFromJsonAsync<List<UserDto>>("api/user", cancellationToken) ?? [];
	}

	public async Task<UserDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var user = await httpClient.GetFromJsonAsync<UserDto>($"api/user/by-name?name={Uri.EscapeDataString(name)}", cancellationToken);
		return user ?? throw new InvalidOperationException("API returned an empty response.");
	}

	public async Task CreateAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsJsonAsync("api/user", user, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateAsync(int id, UpdateUserRequest user, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PutAsJsonAsync($"api/user?id={id}", user, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
	{
		using var request = new HttpRequestMessage(HttpMethod.Put, $"api/user/activate?id={id}");
		var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
	{
		using var request = new HttpRequestMessage(HttpMethod.Put, $"api/user/deactivate?id={id}");
		var response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
	}
}
