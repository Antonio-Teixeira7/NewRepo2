using UserSite.Data.Dtos;

namespace UserSite.Services
{
	public interface IUserService
	{
		Task ActivateAsync(int id);
		Task CreateAsync(UserDto dto);
		Task DeactivateAsync(int id);
		Task<List<UserDto>> GetAllAsync();
		Task<UserDto> GetByNameAsync(string name);
		Task UpdateAsync(int id, UserDto dto);
	}
}