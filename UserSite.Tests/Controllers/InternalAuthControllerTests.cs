using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using UserSite.Configuration;
using UserSite.Controllers;
using UserSite.Data.Dtos;
using UserSite.Services;

namespace UserSite.Tests.Controllers;

public class InternalAuthControllerTests
{
	[Fact]
	public async Task ValidateCredentialsAsync_ChaveInternaInvalida_DeveRetornarUnauthorized()
	{
		// Arrange
		var serviceMock = new Mock<IUserService>();
		var controller = CreateController(serviceMock, "api-key-correta");
		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext()
		};
		controller.ControllerContext.HttpContext.Request.Headers["X-Internal-Api-Key"] = "api-key-invalida";

		// Act
		var result = await controller.ValidateCredentialsAsync(new ValidateCredentialsRequestDto
		{
			Email = "user@test.com",
			Password = "123456"
		});

		// Assert
		Assert.IsType<UnauthorizedResult>(result.Result);
	}

	[Fact]
	public async Task ValidateCredentialsAsync_CredenciaisValidas_DeveRetornarOk()
	{
		// Arrange
		var serviceMock = new Mock<IUserService>();
		serviceMock
			.Setup(s => s.ValidateCredentialsAsync(It.IsAny<ValidateCredentialsRequestDto>()))
			.ReturnsAsync(new ValidateCredentialsResponseDto
			{
				Id = 1,
				Email = "user@test.com",
				IsActive = true
			});

		var controller = CreateController(serviceMock, "api-key-correta");
		controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext()
		};
		controller.ControllerContext.HttpContext.Request.Headers["X-Internal-Api-Key"] = "api-key-correta";

		// Act
		var result = await controller.ValidateCredentialsAsync(new ValidateCredentialsRequestDto
		{
			Email = "user@test.com",
			Password = "123456"
		});

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result.Result);
		var payload = Assert.IsType<ValidateCredentialsResponseDto>(okResult.Value);
		Assert.Equal(1, payload.Id);
		Assert.Equal("user@test.com", payload.Email);
		Assert.True(payload.IsActive);
	}

	private static InternalAuthController CreateController(Mock<IUserService> serviceMock, string internalApiKey)
	{
		return new InternalAuthController(
			serviceMock.Object,
			Options.Create(new AuthIntegrationOptions { InternalApiKey = internalApiKey }));
	}
}
