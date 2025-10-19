using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
	
		public class AdminController : Controller
		{
			private readonly IHttpClientFactory _httpClientFactory;

			public AdminController(IHttpClientFactory httpClientFactory)
			{
				_httpClientFactory = httpClientFactory;
			}

			private bool IsAdmin()
			{
				return HttpContext.Session.GetString("UserRole") == "Admin";
			}

			// GET: Admin/Index (Dashboard)
			[HttpGet]
			public async Task<IActionResult> Index()
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");

					// Get pending users
					var usersResponse = await client.GetAsync(ApiConstants.AdminEndpoints.PendingUsers);
					var pendingUsers = usersResponse.IsSuccessStatusCode
						? await usersResponse.Content.ReadFromJsonAsync<List<UserModel>>() ?? new List<UserModel>()
						: new List<UserModel>();

					// Get pending articles
					var articlesResponse = await client.GetAsync(ApiConstants.AdminEndpoints.PendingArticles);
					var pendingArticles = articlesResponse.IsSuccessStatusCode
						? await articlesResponse.Content.ReadFromJsonAsync<List<ArticleModel>>() ?? new List<ArticleModel>()
						: new List<ArticleModel>();

					var model = new AdminDashboardViewModel
					{
						PendingUsers = pendingUsers,
						PendingArticles = pendingArticles
					};

					return View(model);
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
					return View(new AdminDashboardViewModel());
				}
			}

			// GET: Admin/AllArticlesList
			[HttpGet]
			public async Task<IActionResult> AllArticlesList()
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var response = await client.GetAsync(ApiConstants.ArticleEndpoints.Approved);

					if (response.IsSuccessStatusCode)
					{
						var articles = await response.Content.ReadFromJsonAsync<List<ArticleModel>>()
							?? new List<ArticleModel>();
						return View(articles);
					}

					return View(new List<ArticleModel>());
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
					return View(new List<ArticleModel>());
				}
			}

			// GET: Admin/ApproveUser
			[HttpGet]
			public async Task<IActionResult> ApproveUser(int id)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.AdminEndpoints.ApproveUser, id);
					var response = await client.PutAsync(url, null);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "User approved successfully";
					else
						TempData["ErrorMessage"] = "Failed to approve user";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("Index");
			}

			// GET: Admins/Decline
			[HttpGet]
			public async Task<IActionResult> Decline(int id)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.AdminEndpoints.DeleteUser, id);
					var response = await client.DeleteAsync(url);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "User registration declined";
					else
						TempData["ErrorMessage"] = "Failed to decline user";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("Index");
			}

			// GET: Admin/StaffList
			[HttpGet]
			public async Task<IActionResult> StaffList()
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var response = await client.GetAsync(ApiConstants.AdminEndpoints.StaffMembers);

					if (response.IsSuccessStatusCode)
					{
						var staff = await response.Content.ReadFromJsonAsync<List<StaffModel>>()
							?? new List<StaffModel>();
						return View(staff);
					}

					return View(new List<StaffModel>());
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
					return View(new List<StaffModel>());
				}
			}

			// GET: Admin/CreateStaff
			[HttpGet]
			public IActionResult CreateStaff()
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				return View();
			}

			// POST: Admin/CreateStaff
			[HttpPost]
			[ValidateAntiForgeryToken]
			public async Task<IActionResult> CreateStaff(StaffModel model)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				if (!ModelState.IsValid)
					return View(model);

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var response = await client.PostAsJsonAsync(ApiConstants.AdminEndpoints.AddStaff, model);

					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadFromJsonAsync<ApiResponse<StaffModel>>();
						TempData["SuccessMessage"] = result?.Message ?? "Staff added successfully";
						return RedirectToAction("StaffList");
					}

					ModelState.AddModelError("", "Failed to add staff");
					return View(model);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Error: {ex.Message}");
					return View(model);
				}
			}

			// GET: Admin/DeleteStaff
			[HttpGet]
			public async Task<IActionResult> DeleteStaff(int id)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.AdminEndpoints.DeleteStaff, id);
					var response = await client.DeleteAsync(url);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "Staff deleted successfully";
					else
						TempData["ErrorMessage"] = "Failed to delete staff";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("StaffList");
			}

			// GET: Admin/ApproveArticle
			[HttpGet]
			public async Task<IActionResult> ApproveArticle(int id)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.AdminEndpoints.ApproveArticle, id);
					var response = await client.PutAsync(url, null);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "Article approved successfully";
					else
						TempData["ErrorMessage"] = "Failed to approve article";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("Index");
			}

			// GET: Admin/DeclineArticle
			[HttpGet]
			public async Task<IActionResult> DeclineArticle(int id)
			{
				if (!IsAdmin())
					return RedirectToAction("Login", "Home");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.AdminEndpoints.RejectArticle, id);
					var response = await client.DeleteAsync(url);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "Article rejected successfully";
					else
						TempData["ErrorMessage"] = "Failed to reject article";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("Index");
			}
		}

		public class AdminDashboardViewModel
		{
			public List<UserModel> PendingUsers { get; set; } = new();
			public List<ArticleModel> PendingArticles { get; set; } = new();
		}
	}


