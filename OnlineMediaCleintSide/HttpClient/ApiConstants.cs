namespace OnlineMediaCleintSide.Api
{
	public static class ApiConstants
	{
		public const string BaseUrl = "https://localhost:5027/api/";
		
		// Auth endpoints
		public static class AuthEndpoints
		{
			public const string UserRegister = "Auth/user/register";
			public const string UserLogin = "Auth/user/login";
			public const string UserLogout = "Auth/user/logout";
			public const string StaffLogin = "Auth/staff/login";
			public const string StaffLogout = "Auth/staff/logout";
		}
		
		// User endpoints
		public static class UserEndpoints
		{
			public const string PostArticle = "User/post-article";
			public const string GetMyArticles = "User/my-articles/{0}";
			public const string GetProfile = "User/profile/{0}";
			public const string UpdateProfile = "User/update-profile";
			public const string GetApprovedArticles = "User/approved-articles";
		}
		
		// Admin endpoints
		public static class AdminEndpoints
		{
			public const string PendingUsers = "Admin/pending-users";
			public const string ApproveUser = "Admin/approve-user/{0}";
			public const string DeleteUser = "Admin/delete-user/{0}";
			public const string PendingArticles = "Admin/pending-articles";
			public const string ApproveArticle = "Admin/approve-article/{0}";
			public const string RejectArticle = "Admin/reject-article/{0}";
			public const string StaffMembers = "Admin/staff-members";
			public const string AddStaff = "Admin/add-staff";
			public const string DeleteStaff = "Admin/delete-staff/{0}";
			public const string GetAllAdvertisements = "Admin/advertisements";
		}
		
		// Article endpoints
		public static class ArticleEndpoints
		{
			public const string Approved = "Article/approved";
			public const string GetById = "Article/{0}";
			public const string Update = "Article/update";
			public const string Delete = "Article/delete/{0}";
		}
		
		// Staff endpoints
		public static class StaffEndpoints
		{
			public const string Advertisements = "Staff/advertisements";
			public const string ViewUsers = "Staff/view-users";
			public const string AddAdvertisement = "Staff/add-advertisement";
			public const string UpdateAdvertisement = "Staff/update-advertisement";
			public const string DeleteAdvertisement = "Staff/delete-advertisement/{0}";
		}
		
		// Advertisement endpoints
		public static class AdvertisementEndpoints
		{
			public const string All = "Advertisement/all";
			public const string GetById = "Advertisement/{0}";
		}
	}
}
