using System.ComponentModel.DataAnnotations;

namespace OnlineMediaCleintSide.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string UserName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(100)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string LastName { get; set; } = string.Empty;

		[Required]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; } = string.Empty;

		public long PhoneNumber { get; set; }

		[StringLength(20)]
		public string Role { get; set; } = "User";

		public bool IsApproved { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
