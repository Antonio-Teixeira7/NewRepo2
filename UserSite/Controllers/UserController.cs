using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSite.Data.Dtos;
using UserSite.Services;

namespace UserSite.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task CreateAsync([FromBody] CreateUserDto dto)
	{
		await _userService.CreateAsync(dto);
	}

	[HttpGet]
	public async Task<List<UserDto>> GetAllAsync()
	{
		return await _userService.GetAllAsync();
	}

	[HttpGet("by-name")]
	public async Task<UserDto> GetByNameAsync([FromQuery] string name)
	{
		return await _userService.GetByNameAsync(name);
	}

	[HttpPut]
	public async Task UpdateAsync([FromQuery] int id, [FromBody] UpdateUserDto dto)
	{
		await _userService.UpdateAsync(id, dto);
	}

	[HttpPut("deactivate")]
	public async Task DeactivateAsync([FromQuery] int id)
	{
		await _userService.DeactivateAsync(id);
	}

	[HttpPut("activate")]
	public async Task ActivateAsync([FromQuery] int id)
	{
		await _userService.ActivateAsync(id);
	}
}
