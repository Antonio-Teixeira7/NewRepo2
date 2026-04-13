using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserSite.Configuration;
using UserSite.Data.Dtos;
using UserSite.Services;

namespace UserSite.Controllers;

[ApiController]
[Route("api/internal/auth")]
public class InternalAuthController : ControllerBase
{
	private const string InternalApiKeyHeaderName = "X-Internal-Api-Key";

	private readonly IUserService _userService;
	private readonly AuthIntegrationOptions _authIntegrationOptions;

	public InternalAuthController(IUserService userService, IOptions<AuthIntegrationOptions> authIntegrationOptions)
	{
		_userService = userService;
		_authIntegrationOptions = authIntegrationOptions.Value;
	}

	[AllowAnonymous]
	[HttpPost("validate-credentials")]
	public async Task<ActionResult<ValidateCredentialsResponseDto>> ValidateCredentialsAsync([FromBody] ValidateCredentialsRequestDto dto)
	{
		if (!Request.Headers.TryGetValue(InternalApiKeyHeaderName, out var providedApiKey) ||
			string.IsNullOrWhiteSpace(_authIntegrationOptions.InternalApiKey))
		{
			return Unauthorized();
		}

		var providedApiKeyValue = providedApiKey.ToString();
		if (!string.Equals(providedApiKeyValue, _authIntegrationOptions.InternalApiKey, StringComparison.Ordinal))
		{
			return Unauthorized();
		}

		var user = await _userService.ValidateCredentialsAsync(dto);
		if (user is null)
		{
			return Unauthorized();
		}

		return Ok(user);
	}
}
