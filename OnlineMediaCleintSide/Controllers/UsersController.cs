using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
	public class UsersController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public UsersController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		// GET: Users
		public IActionResult Index()
		{
			return View();
		}

		// GET: Users/Edit/5
		public IActionResult Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			return View();
		}

		// POST: Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, UserModel model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var response = await client.PutAsJsonAsync($"User/update/{id}", model);

					if (response.IsSuccessStatusCode)
					{
						TempData["SuccessMessage"] = "Profile updated successfully";
						return RedirectToAction(nameof(Index));
					}
					else
					{
						ModelState.AddModelError("", "Failed to update profile");
					}
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Error: {ex.Message}");
				}
			}

			return View(model);
		}
	}
}