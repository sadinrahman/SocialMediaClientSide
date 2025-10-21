using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
		public class AdvertisementController : Controller
		{
			private readonly IHttpClientFactory _httpClientFactory;

			public AdvertisementController(IHttpClientFactory httpClientFactory)
			{
				_httpClientFactory = httpClientFactory;
			}

			private bool IsStaff()
			{
				var role = HttpContext.Session.GetString("UserRole");
				return role == "Staff" || role == "Admin";
			}

			private int GetCurrentStaffId()
			{
				var staffIdString = HttpContext.Session.GetString("UserId");
				return int.TryParse(staffIdString, out var staffId) ? staffId : 0;
			}

			// GET: Advertisement/Index
			[HttpGet]
			public async Task<IActionResult> Index()
			{
				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var response = await client.GetAsync(ApiConstants.AdvertisementEndpoints.All);

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

			// GET: Advertisement/Details/5
			[HttpGet]
			public async Task<IActionResult> Details(int id)
			{
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

			// GET: Advertisement/Create
			[HttpGet]
			public IActionResult Create()
			{
				if (!IsStaff())
					return RedirectToAction("UserLogin", "Login");

				return View();
			}

			// POST: Advertisement/Create
			[HttpPost]
			[ValidateAntiForgeryToken]
			public async Task<IActionResult> Create(AdvertisementModel model)
			{
				if (!IsStaff())
					return RedirectToAction("UserLogin", "Login");

				if (!ModelState.IsValid)
					return View(model);

				try
				{
					model.StaffId = GetCurrentStaffId();
					var client = _httpClientFactory.CreateClient("BackendAPI");

					// Handle file upload
					if (model.ImageFile != null)
					{
						using var content = new MultipartFormDataContent();
						content.Add(new StringContent(model.Title), "Title");
						content.Add(new StringContent(model.Description), "Description");
						content.Add(new StringContent(model.StaffId.ToString()), "StaffId");

						if (!string.IsNullOrEmpty(model.Category))
						{
							content.Add(new StringContent(model.Category), "Category");
						}

						var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
						streamContent.Headers.ContentType =
							new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
						content.Add(streamContent, "ImageFile", model.ImageFile.FileName);

						var response = await client.PostAsync(ApiConstants.StaffEndpoints.AddAdvertisement, content);

						if (response.IsSuccessStatusCode)
						{
							var result = await response.Content.ReadFromJsonAsync<ApiResponse<AdvertisementModel>>();
							TempData["SuccessMessage"] = result?.Message ?? "Advertisement posted successfully";
							return RedirectToAction("Index", "Staff");
						}
					}
					else
					{
						var response = await client.PostAsJsonAsync(ApiConstants.StaffEndpoints.AddAdvertisement, model);

						if (response.IsSuccessStatusCode)
						{
							var result = await response.Content.ReadFromJsonAsync<ApiResponse<AdvertisementModel>>();
							TempData["SuccessMessage"] = result?.Message ?? "Advertisement posted successfully";
							return RedirectToAction("Index", "Staff");
						}
					}

					ModelState.AddModelError("", "Failed to create advertisement");
					return View(model);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", $"Error: {ex.Message}");
					return View(model);
				}
			}

			// GET: Advertisement/Edit/5
			[HttpGet]
			public async Task<IActionResult> Edit(int id)
			{
				if (!IsStaff())
					return RedirectToAction("UserLogin", "Login");

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
					return RedirectToAction("Index", "Staff");
				}
			}

			// POST: Advertisement/Edit/5
			[HttpPost]
			[ValidateAntiForgeryToken]
			public async Task<IActionResult> Edit(int id, AdvertisementModel model)
			{
				if (!IsStaff())
					return RedirectToAction("UserLogin", "Login");

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
						return RedirectToAction("Index", "Staff");
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

			// GET: Advertisement/Delete/5
			[HttpGet]
			public async Task<IActionResult> Delete(int id)
			{
				if (!IsStaff())
					return RedirectToAction("UserLogin", "Login");

				try
				{
					var client = _httpClientFactory.CreateClient("BackendAPI");
					var url = string.Format(ApiConstants.StaffEndpoints.DeleteAdvertisement, id);
					var response = await client.DeleteAsync(url);

					if (response.IsSuccessStatusCode)
						TempData["SuccessMessage"] = "Advertisement deleted successfully";
					else
						TempData["ErrorMessage"] = "Failed to delete advertisement";
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error: {ex.Message}";
				}

				return RedirectToAction("Index", "Staffs");
			}
		}
}
