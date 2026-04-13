namespace UserSite.Data.Dtos;

public class ValidateCredentialsResponseDto
{
	public int Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public bool IsActive { get; set; }
}
