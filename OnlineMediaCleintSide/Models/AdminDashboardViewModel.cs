namespace OnlineMediaCleintSide.Models
{
	public class AdminDashboardViewModel
	{
		public List<UserModel> PendingUsers { get; set; } = new();
		public List<ArticleModel> PendingArticles { get; set; } = new();
	}
}
