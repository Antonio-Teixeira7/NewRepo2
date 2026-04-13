using UserSite.Data;
using UserSite.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserSite.Repositories;

public class UserRepository : IUserRepository
{
	private readonly UserDbContext _context;

	public UserRepository(UserDbContext dbContext)
	{
		_context = dbContext;
	}

	public async Task CreateAsync(User user)
	{
		await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();
	}

	public async Task<List<User>> GetAllAsync()
	{
		return await _context.Users
			.AsNoTracking()
			.OrderBy(u => u.Id)
			.ToListAsync();
	}

	public async Task<User?> GetByNameAsync(string name)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Name == name);
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<User?> GetByIdAsync(int id)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task UpdateAsync(User user)
	{
		await _context.SaveChangesAsync();
	}

	public async Task DeactivateAsync(int id)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user is null)
		{
			return;
		}

		user.IsActive = false;
		await _context.SaveChangesAsync();
	}

	public async Task ActivateAsync(int id)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user is null)
		{
			return;
		}

		user.IsActive = true;
		await _context.SaveChangesAsync();
	}
}
