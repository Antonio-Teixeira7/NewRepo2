using System.ComponentModel.DataAnnotations;

namespace UserSite.Data.Dtos;

public class ValidateCredentialsRequestDto
{
	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	[Required]
	public string Password { get; set; } = string.Empty;
}
