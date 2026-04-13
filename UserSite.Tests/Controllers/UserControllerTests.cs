using Moq;
using UserSite.Controllers;
using UserSite.Data.Dtos;
using UserSite.Services;

namespace UserSite.Tests.Controllers;

public class UserControllerTests
{
	[Fact]
	public async Task GetAllAsync_ServiceRetornaUsuarios_DeveRetornarLista()
	{
		// Arrange
		var serviceMock = new Mock<IUserService>();
		var expected = new List<UserDto>
		{
			new() { Name = "A", Email = "a@test.com" },
			new() { Name = "B", Email = "b@test.com" }
		};

		serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(expected);
		var controller = new UserController(serviceMock.Object);

		// Act
		var result = await controller.GetAllAsync();

		// Assert
		Assert.Equal(expected.Count, result.Count);
		Assert.Equal(expected[0].Name, result[0].Name);
		Assert.Equal(expected[0].Email, result[0].Email);
		Assert.Equal(expected[1].Name, result[1].Name);
		Assert.Equal(expected[1].Email, result[1].Email);
	}

	[Fact]
	public async Task CreateAsync_DtoValido_DeveChamarServicoUmaVez()
	{
		// Arrange
		var serviceMock = new Mock<IUserService>();
		var controller = new UserController(serviceMock.Object);
		var dto = new CreateUserDto { Name = "Antonio", Email = "antonio@test.com", Password = "123456" };

		// Act
		await controller.CreateAsync(dto);

		// Assert
		serviceMock.Verify(s => s.CreateAsync(dto), Times.Once);
	}

	[Fact]
	public async Task GetByNameAsync_NomeValido_DeveRetornarUsuario()
	{
		// Arrange
		const string name = "Maria";
		var serviceMock = new Mock<IUserService>();
		var expected = new UserDto { Name = name, Email = "maria@test.com" };

		serviceMock.Setup(s => s.GetByNameAsync(name)).ReturnsAsync(expected);
		var controller = new UserController(serviceMock.Object);

		// Act
		var result = await controller.GetByNameAsync(name);

		// Assert
		Assert.Equal(expected.Name, result.Name);
		Assert.Equal(expected.Email, result.Email);
	}
}
