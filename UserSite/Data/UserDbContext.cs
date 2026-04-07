using Microsoft.EntityFrameworkCore;
using UserSite.Data.Entities;

namespace UserSite.Data;

public class UserDbContext : DbContext
{
	public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
	{
		
	}

	public DbSet<User> Users { get; set; } = null!;
}
