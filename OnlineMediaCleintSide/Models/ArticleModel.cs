using System.ComponentModel.DataAnnotations;

namespace OnlineMediaCleintSide.Models
{
	public class ArticleModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; } = string.Empty;

		[Required]
		public string Body { get; set; } = string.Empty;

		public string ImagePath { get; set; } = string.Empty;

		[StringLength(500)]
		public string Summary { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string Category { get; set; } = string.Empty;

		[StringLength(100)]
		public string Topic { get; set; } = string.Empty;

		[Display(Name = "Image File")]
		public IFormFile? ImageFile { get; set; }

		public int UserId { get; set; }

		[StringLength(100)]
		public string? AuthorName { get; set; }

		public bool IsApproved { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }
	}
}
