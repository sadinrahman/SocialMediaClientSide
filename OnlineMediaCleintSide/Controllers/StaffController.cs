using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
	public class StaffsController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public StaffsController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		private bool IsStaff()
		{
			var role = HttpContext.Session.GetString("UserRole");
			return role == "Staff" || role == "Admin";
		}

		// GET: Staffs/Index
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!IsStaff())
				return RedirectToAction("Login", "Home");

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.GetAsync(ApiConstants.StaffEndpoints.Advertisements);

				if (response.IsSuccessStatusCode)
				{
					var advertisements = await response.Content.ReadFromJsonAsync<List<AdvertisementModel>>()
						?? new List<AdvertisementModel>();
					return View(advertisements);
				}

				return View(new List<AdvertisementModel>());
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return View(new List<AdvertisementModel>());
			}
		}

		// GET: Staffs/Details (View Users)
		[HttpGet]
		public async Task<IActionResult> Details()
		{
			if (!IsStaff())
				return RedirectToAction("Login", "Home");

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.GetAsync(ApiConstants.StaffEndpoints.ViewUsers);

				if (response.IsSuccessStatusCode)
				{
					var users = await response.Content.ReadFromJsonAsync<List<UserModel>>()
						?? new List<UserModel>();
					return View(users);
				}

				return View(new List<UserModel>());
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return View(new List<UserModel>());
			}
		}

		// GET: Staffs/Edit (Edit Advertisement)
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (!IsStaff())
				return RedirectToAction("Login", "Home");

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var url = string.Format(ApiConstants.AdvertisementEndpoints.GetById, id);
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var advertisement = await response.Content.ReadFromJsonAsync<AdvertisementModel>();
					return View(advertisement);
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return RedirectToAction("Index");
			}
		}

		// POST: Staffs/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, AdvertisementModel model)
		{
			if (!IsStaff())
				return RedirectToAction("Login", "Home");

			if (id != model.Id)
				return NotFound();

			if (!ModelState.IsValid)
				return View(model);

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.PutAsJsonAsync(ApiConstants.StaffEndpoints.UpdateAdvertisement, model);

				if (response.IsSuccessStatusCode)
				{
					TempData["SuccessMessage"] = "Advertisement updated successfully";
					return RedirectToAction("Index");
				}

				ModelState.AddModelError("", "Failed to update advertisement");
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error: {ex.Message}");
				return View(model);
			}
		}
	}
}
