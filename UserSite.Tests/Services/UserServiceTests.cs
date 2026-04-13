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
		var dto = new CreateUserDto { Name = "Antonio", Email = "antonio@test.com", Password = "123456" };
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
		Assert.NotEqual(dto.Password, capturedUser.PasswordHash);
		Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, capturedUser.PasswordHash));
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

	[Fact]
	public async Task UpdateAsync_SemNovaSenha_DeveManterHashAtual()
	{
		// Arrange
		var currentHash = BCrypt.Net.BCrypt.HashPassword("senha-atual");
		var user = new User
		{
			Id = 10,
			Name = "Antonio",
			Email = "old@test.com",
			PasswordHash = currentHash,
			IsActive = true
		};
		var dto = new UpdateUserDto
		{
			Name = "Antonio Novo",
			Email = "new@test.com",
			Password = null
		};

		_repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
		_repositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

		// Act
		await _service.UpdateAsync(user.Id, dto);

		// Assert
		Assert.Equal(dto.Name, user.Name);
		Assert.Equal(dto.Email, user.Email);
		Assert.Equal(currentHash, user.PasswordHash);
		_repositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_ComNovaSenha_DeveGerarNovoHash()
	{
		// Arrange
		var currentHash = BCrypt.Net.BCrypt.HashPassword("senha-antiga");
		var user = new User
		{
			Id = 11,
			Name = "Maria",
			Email = "maria@test.com",
			PasswordHash = currentHash,
			IsActive = true
		};
		var dto = new UpdateUserDto
		{
			Name = "Maria",
			Email = "maria@test.com",
			Password = "nova-senha"
		};

		_repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
		_repositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

		// Act
		await _service.UpdateAsync(user.Id, dto);

		// Assert
		Assert.NotEqual(currentHash, user.PasswordHash);
		Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash));
		_repositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
	}

	[Fact]
	public async Task ValidateCredentialsAsync_CredenciaisValidas_DeveRetornarUsuarioSeguro()
	{
		// Arrange
		const string password = "senha-segura";
		var dto = new ValidateCredentialsRequestDto
		{
			Email = "auth@test.com",
			Password = password
		};
		var user = new User
		{
			Id = 7,
			Name = "Auth",
			Email = dto.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
			IsActive = true
		};

		_repositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

		// Act
		var result = await _service.ValidateCredentialsAsync(dto);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(user.Id, result!.Id);
		Assert.Equal(user.Email, result.Email);
		Assert.True(result.IsActive);
	}

	[Fact]
	public async Task ValidateCredentialsAsync_SenhaInvalida_DeveRetornarNulo()
	{
		// Arrange
		var dto = new ValidateCredentialsRequestDto
		{
			Email = "auth@test.com",
			Password = "senha-errada"
		};
		var user = new User
		{
			Id = 8,
			Name = "Auth",
			Email = dto.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha-correta"),
			IsActive = true
		};

		_repositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

		// Act
		var result = await _service.ValidateCredentialsAsync(dto);

		// Assert
		Assert.Null(result);
	}
}
