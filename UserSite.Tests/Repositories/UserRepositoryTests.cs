using Microsoft.EntityFrameworkCore;
using UserSite.Data;
using UserSite.Data.Dtos;
using UserSite.Data.Entities;
using UserSite.Repositories;

namespace UserSite.Tests.Repositories;

public class UserRepositoryTests
{
	private static UserDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<UserDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		return new UserDbContext(options);
	}

	[Fact]
	public async Task GetAllAsync_QuandoExistemAtivosEInativos_DeveRetornarApenasAtivos()
	{
		// Arrange
		await using var context = CreateContext();
		context.Users.AddRange(
			new User { Name = "Ativo", Email = "ativo@test.com", IsActive = true },
			new User { Name = "Inativo", Email = "inativo@test.com", IsActive = false });
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);

		// Act
		var users = await repository.GetAllAsync();

		// Assert
		Assert.Single(users);
		Assert.Equal("Ativo", users[0].Name);
	}

	[Fact]
	public async Task UpdateAsync_UsuarioExiste_DeveAtualizarNomeEEmail()
	{
		// Arrange
		await using var context = CreateContext();
		var user = new User { Name = "Nome Antigo", Email = "old@test.com", IsActive = true };
		context.Users.Add(user);
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);
		var dto = new UserDto { Name = "Nome Novo", Email = "new@test.com" };

		// Act
		await repository.UpdateAsync(user.Id, dto);
		var updatedUser = await context.Users.FirstAsync(u => u.Id == user.Id);

		// Assert
		Assert.Equal("Nome Novo", updatedUser.Name);
		Assert.Equal("new@test.com", updatedUser.Email);
		Assert.True(updatedUser.IsActive);
	}

	[Fact]
	public async Task DeactivateAsync_UsuarioExiste_DeveMarcarComoInativo()
	{
		// Arrange
		await using var context = CreateContext();
		var user = new User { Name = "Usuario", Email = "user@test.com", IsActive = true };
		context.Users.Add(user);
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);

		// Act
		await repository.DeactivateAsync(user.Id);
		var updatedUser = await context.Users.FirstAsync(u => u.Id == user.Id);

		// Assert
		Assert.False(updatedUser.IsActive);
	}

	[Fact]
	public async Task ActivateAsync_UsuarioInativo_DeveMarcarComoAtivo()
	{
		// Arrange
		await using var context = CreateContext();
		var user = new User { Name = "Usuario", Email = "user@test.com", IsActive = false };
		context.Users.Add(user);
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);

		// Act
		await repository.ActivateAsync(user.Id);
		var updatedUser = await context.Users.FirstAsync(u => u.Id == user.Id);

		// Assert
		Assert.True(updatedUser.IsActive);
	}
}
