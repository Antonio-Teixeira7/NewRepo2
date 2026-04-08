using UserSite.Data.Dtos;
using UserSite.Data.Entities;
using UserSite.Repositories;

namespace UserSite.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository)
	{
		_repository = repository;
	}

	public async Task CreateAsync(UserDto dto)
	{
		User user = new() { Name = dto.Name, Email = dto.Email, IsActive = false };

		await _repository.CreateAsync(user);
	}

	public async Task<List<UserDto>> GetAllAsync()
	{
		var users = await _repository.GetAllAsync();
		return users
			.Select(user => new UserDto { Name = user.Name, Email = user.Email })
			.ToList();
	}

	public async Task<UserDto> GetByNameAsync(string name)
	{
		var user = await _repository.GetByNameAsync(name);
		if (user is null)
		{
			throw new KeyNotFoundException($"User '{name}' was not found.");
		}

		UserDto dto = new() { Name = user.Name, Email = user.Email };

		return dto;
	}

	public async Task UpdateAsync(int id, UserDto dto)
	{
		await _repository.UpdateAsync(id, dto);
	}

	public async Task DeactivateAsync(int id)
	{
		await _repository.DeactivateAsync(id);
	}

	public async Task ActivateAsync(int id)
	{
		await _repository.ActivateAsync(id);
	}
}
