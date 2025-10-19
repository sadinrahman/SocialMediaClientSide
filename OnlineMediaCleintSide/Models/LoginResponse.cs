namespace OnlineMediaCleintSide.Models
{
	public class LoginResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public UserData? Data { get; set; }
	}

	public class UserData
	{
		public int Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public string? Designation { get; set; }
	}
}
