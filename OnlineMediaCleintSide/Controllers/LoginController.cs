using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
	public class LoginController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public LoginController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		// GET: Login/Register
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		// POST: Login/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.PostAsJsonAsync(ApiConstants.AuthEndpoints.UserRegister, model);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserModel>>();
					TempData["SuccessMessage"] = result?.Message ?? "Registration submitted. Please wait for admin approval.";
					return RedirectToAction("Login", "Home");
				}

				var error = await response.Content.ReadAsStringAsync();
				ModelState.AddModelError("", $"Registration failed: {error}");
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error: {ex.Message}");
				return View(model);
			}
		}

		// POST: Login/UserLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UserLogin(LoginRequest model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Login", "Home");
			}

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.PostAsJsonAsync(ApiConstants.AuthEndpoints.UserLogin, model);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

					if (result?.Data != null)
					{
						// Store user info in session
						HttpContext.Session.SetString("UserId", result.Data.Id.ToString());
						HttpContext.Session.SetString("UserName", result.Data.UserName);
						HttpContext.Session.SetString("FullName", $"{result.Data.FirstName} {result.Data.LastName}");
						HttpContext.Session.SetString("UserRole", result.Data.Role);

						TempData["SuccessMessage"] = result.Message;

						// Redirect to user dashboard
						return RedirectToAction("Index", "Users");
					}
				}

				TempData["ErrorMessage"] = "Invalid login attempt or account not approved";
				return RedirectToAction("Login", "Home");
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return RedirectToAction("Login", "Home");
			}
		}

		// POST: Login/StaffLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> StaffLogin(LoginRequest model)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Login", "Home");
			}

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.PostAsJsonAsync(ApiConstants.AuthEndpoints.StaffLogin, model);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

					if (result?.Data != null)
					{
						// Store staff info in session
						HttpContext.Session.SetString("UserId", result.Data.Id.ToString());
						HttpContext.Session.SetString("UserName", result.Data.UserName);
						HttpContext.Session.SetString("FullName", $"{result.Data.FirstName} {result.Data.LastName}");
						HttpContext.Session.SetString("UserRole", result.Data.Role);

						if (!string.IsNullOrEmpty(result.Data.Designation))
						{
							HttpContext.Session.SetString("Designation", result.Data.Designation);
						}

						TempData["SuccessMessage"] = result.Message;

						// Redirect based on role
						return result.Data.Role switch
						{
							"Admin" => RedirectToAction("Index", "Admin"),
							"Staff" => RedirectToAction("Index", "Staff"),
							_ => RedirectToAction("Login", "Home")
						};
					}
				}

				TempData["ErrorMessage"] = "Invalid staff credentials";
				return RedirectToAction("Login", "Home");
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return RedirectToAction("Login", "Home");
			}
		}

		// GET: Login/Logout
		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var role = HttpContext.Session.GetString("UserRole");

				var endpoint = (role == "Admin" || role == "Staff")
					? ApiConstants.AuthEndpoints.StaffLogout
					: ApiConstants.AuthEndpoints.UserLogout;

				await client.PostAsync(endpoint, null);
			}
			catch
			{
				// Continue with logout even if API call fails
			}
			finally
			{
				HttpContext.Session.Clear();
			}

			return RedirectToAction("Login", "Home");
		}
	}
}
