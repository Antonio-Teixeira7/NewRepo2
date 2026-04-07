using UserSite.Data.Dtos;
using UserSite.Data.Entities;

namespace UserSite.Repositories
{
	public interface IUserRepository
	{
		Task ActivateAsync(int id);
		Task CreateAsync(User user);
		Task DeactivateAsync(int id);
		Task<List<User>> GetAllAsync();
		Task<User?> GetByNameAsync(string name);
		Task UpdateAsync(int id, UserDto dto);
	}
}