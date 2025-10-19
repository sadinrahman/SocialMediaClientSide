using System.ComponentModel.DataAnnotations;

namespace OnlineMediaCleintSide.Models
{
	public class StaffModel
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

		[Required]
		[StringLength(50)]
		public string Designation { get; set; } = string.Empty;

		[StringLength(20)]
		public string Role { get; set; } = "Staff";

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
