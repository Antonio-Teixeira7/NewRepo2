using Moq;
using UserSite.Data.Dtos;
using UserSite.Data.Entities;
using UserSite.Repositories;
using UserSite.Services;

namespace UserSite.Tests.Services;

public class UserServiceTests
{
	private readonly Mock<IUserRepository> _repositoryMock;
	private readonly UserService _service;

	public UserServiceTests()
	{
		_repositoryMock = new Mock<IUserRepository>();
		_service = new UserService(_repositoryMock.Object);
	}

	[Fact]
	public async Task CreateAsync_DtoValido_DeveCriarUsuarioAtivo()
	{
		// Arrange
		var dto = new UserDto { Name = "Antonio", Email = "antonio@test.com" };
		User? capturedUser = null;

		_repositoryMock
			.Setup(r => r.CreateAsync(It.IsAny<User>()))
			.Callback<User>(user => capturedUser = user)
			.Returns(Task.CompletedTask);

		// Act
		await _service.CreateAsync(dto);

		// Assert
		_repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
		Assert.NotNull(capturedUser);
		Assert.Equal(dto.Name, capturedUser!.Name);
		Assert.Equal(dto.Email, capturedUser.Email);
		Assert.True(capturedUser.IsActive);
	}

	[Fact]
	public async Task GetByNameAsync_UsuarioExiste_DeveRetornarDto()
	{
		// Arrange
		const string name = "Maria";
		_repositoryMock
			.Setup(r => r.GetByNameAsync(name))
			.ReturnsAsync(new User
			{
				Id = 1,
				Name = name,
				Email = "maria@test.com",
				IsActive = true
			});

		// Act
		var result = await _service.GetByNameAsync(name);

		// Assert
		Assert.Equal(1, result.Id);
		Assert.Equal(name, result.Name);
		Assert.Equal("maria@test.com", result.Email);
		Assert.True(result.IsActive);
	}

	[Fact]
	public async Task GetByNameAsync_UsuarioNaoExiste_DeveLancarKeyNotFoundException()
	{
		// Arrange
		const string name = "Inexistente";
		_repositoryMock
			.Setup(r => r.GetByNameAsync(name))
			.ReturnsAsync((User?)null);

		// Act
		async Task Act() => await _service.GetByNameAsync(name);

		// Assert
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(Act);
		Assert.Equal($"User '{name}' was not found.", exception.Message);
	}

	[Fact]
	public async Task DeactivateAsync_IdValido_DeveChamarRepositorio()
	{
		// Arrange
		const int userId = 10;

		// Act
		await _service.DeactivateAsync(userId);

		// Assert
		_repositoryMock.Verify(r => r.DeactivateAsync(userId), Times.Once);
	}
}
