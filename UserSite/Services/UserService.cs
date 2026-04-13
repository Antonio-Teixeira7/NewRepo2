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

	public async Task CreateAsync(CreateUserDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Password))
		{
			throw new ArgumentException("Password is required.", nameof(dto));
		}

		User user = new()
		{
			Name = dto.Name,
			Email = dto.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
			IsActive = true
		};

		await _repository.CreateAsync(user);
	}

	public async Task<List<UserDto>> GetAllAsync()
	{
		var users = await _repository.GetAllAsync();
		return users
			.Select(user => new UserDto
			{
				Id = user.Id,
				Name = user.Name,
				Email = user.Email,
				IsActive = user.IsActive
			})
			.ToList();
	}

	public async Task<UserDto> GetByNameAsync(string name)
	{
		var user = await _repository.GetByNameAsync(name);
		if (user is null)
		{
			throw new KeyNotFoundException($"User '{name}' was not found.");
		}

		UserDto dto = new()
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			IsActive = user.IsActive
		};

		return dto;
	}

	public async Task UpdateAsync(int id, UpdateUserDto dto)
	{
		var user = await _repository.GetByIdAsync(id);
		if (user is null)
		{
			return;
		}

		user.Name = dto.Name;
		user.Email = dto.Email;

		if (!string.IsNullOrWhiteSpace(dto.Password))
		{
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
		}

		await _repository.UpdateAsync(user);
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
