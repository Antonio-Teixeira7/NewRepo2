using Microsoft.EntityFrameworkCore;
using UserSite.Data;
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
	public async Task GetAllAsync_QuandoExistemAtivosEInativos_DeveRetornarTodos()
	{
		// Arrange
		await using var context = CreateContext();
		context.Users.AddRange(
			new User { Name = "Ativo", Email = "ativo@test.com", PasswordHash = "hash-1", IsActive = true },
			new User { Name = "Inativo", Email = "inativo@test.com", PasswordHash = "hash-2", IsActive = false });
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);

		// Act
		var users = await repository.GetAllAsync();

		// Assert
		Assert.Equal(2, users.Count);
		Assert.Equal("Ativo", users[0].Name);
		Assert.Equal("Inativo", users[1].Name);
	}

	[Fact]
	public async Task UpdateAsync_UsuarioExiste_DeveAtualizarNomeEEmail()
	{
		// Arrange
		await using var context = CreateContext();
		var user = new User { Name = "Nome Antigo", Email = "old@test.com", PasswordHash = "hash-antigo", IsActive = true };
		context.Users.Add(user);
		await context.SaveChangesAsync();
		var repository = new UserRepository(context);
		user.Name = "Nome Novo";
		user.Email = "new@test.com";

		// Act
		await repository.UpdateAsync(user);
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
		var user = new User { Name = "Usuario", Email = "user@test.com", PasswordHash = "hash-user", IsActive = true };
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
		var user = new User { Name = "Usuario", Email = "user@test.com", PasswordHash = "hash-user", IsActive = false };
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
