using System.ComponentModel.DataAnnotations;

namespace OnlineMediaCleintSide.Models
{
	public class AdvertisementModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; } = string.Empty;

		[Required]
		public string Description { get; set; } = string.Empty;

		public string? ImageUrl { get; set; }

		[Display(Name = "Image File")]
		public IFormFile? ImageFile { get; set; }

		public int StaffId { get; set; }

		[StringLength(100)]
		public string? StaffName { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }
	}
}
