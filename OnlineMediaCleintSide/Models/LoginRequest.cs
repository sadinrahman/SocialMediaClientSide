using System.ComponentModel.DataAnnotations;

namespace OnlineMediaCleintSide.Models
{
	public class LoginRequest
	{
		[Required]
		[StringLength(50)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; } = string.Empty;
	}
}
