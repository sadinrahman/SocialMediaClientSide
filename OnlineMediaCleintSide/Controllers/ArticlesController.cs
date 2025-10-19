using Microsoft.AspNetCore.Mvc;
using OnlineMediaCleintSide.Api;
using OnlineMediaCleintSide.Models;

namespace OnlineMediaCleintSide.Controllers
{
	public class ArticlesController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ArticlesController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		private bool IsLoggedIn()
		{
			return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
		}

		private int GetCurrentUserId()
		{
			var userIdString = HttpContext.Session.GetString("UserId");
			return int.TryParse(userIdString, out var userId) ? userId : 0;
		}

		// GET: Articles/Index
		[HttpGet]
		public async Task<IActionResult> Index()
		{
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

		// GET: Articles/Details/5
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var url = string.Format(ApiConstants.ArticleEndpoints.GetById, id);
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var article = await response.Content.ReadFromJsonAsync<ArticleModel>();
					return View(article);
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return RedirectToAction("Index");
			}
		}

		// GET: Articles/Create
		[HttpGet]
		public IActionResult Create()
		{
			if (!IsLoggedIn())
				return RedirectToAction("Login", "Home");

			return View();
		}

		// POST: Articles/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ArticleModel model)
		{
			if (!IsLoggedIn())
				return RedirectToAction("Login", "Home");

			if (!ModelState.IsValid)
				return View(model);

			try
			{
				model.UserId = GetCurrentUserId();
				var client = _httpClientFactory.CreateClient("BackendAPI");

				// Handle file upload
				if (model.ImageFile != null)
				{
					using var content = new MultipartFormDataContent();
					content.Add(new StringContent(model.Title), "Title");
					content.Add(new StringContent(model.Body), "Body");
					content.Add(new StringContent(model.UserId.ToString()), "UserId");

					if (!string.IsNullOrEmpty(model.Category))
					{
						content.Add(new StringContent(model.Category), "Category");
					}

					if (!string.IsNullOrEmpty(model.Summary))
					{
						content.Add(new StringContent(model.Summary), "Summary");
					}

					if (!string.IsNullOrEmpty(model.Topic))
					{
						content.Add(new StringContent(model.Topic), "Topic");
					}

					var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
					streamContent.Headers.ContentType =
						new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
					content.Add(streamContent, "ImageFile", model.ImageFile.FileName);

					var response = await client.PostAsync(ApiConstants.UserEndpoints.PostArticle, content);

					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleModel>>();
						TempData["SuccessMessage"] = result?.Message ?? "Article submitted for approval";
						return RedirectToAction("Index", "Users");
					}
				}
				else
				{
					var response = await client.PostAsJsonAsync(ApiConstants.UserEndpoints.PostArticle, model);

					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadFromJsonAsync<ApiResponse<ArticleModel>>();
						TempData["SuccessMessage"] = result?.Message ?? "Article submitted for approval";
						return RedirectToAction("Index", "Users");
					}
				}

				ModelState.AddModelError("", "Failed to post article");
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error: {ex.Message}");
				return View(model);
			}
		}

		// GET: Articles/Edit/5
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			if (!IsLoggedIn())
				return RedirectToAction("Login", "Home");

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var url = string.Format(ApiConstants.ArticleEndpoints.GetById, id);
				var response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					var article = await response.Content.ReadFromJsonAsync<ArticleModel>();
					return View(article);
				}

				return NotFound();
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
				return RedirectToAction("Index", "Users");
			}
		}

		// POST: Articles/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ArticleModel model)
		{
			if (!IsLoggedIn())
				return RedirectToAction("Login", "Home");

			if (id != model.Id)
				return NotFound();

			if (!ModelState.IsValid)
				return View(model);

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var response = await client.PutAsJsonAsync(ApiConstants.ArticleEndpoints.Update, model);

				if (response.IsSuccessStatusCode)
				{
					TempData["SuccessMessage"] = "Article updated successfully";
					return RedirectToAction("Index", "Users");
				}

				ModelState.AddModelError("", "Failed to update article");
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Error: {ex.Message}");
				return View(model);
			}
		}

		// GET: Articles/Delete/5
		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			if (!IsLoggedIn())
				return RedirectToAction("Login", "Home");

			try
			{
				var client = _httpClientFactory.CreateClient("BackendAPI");
				var url = string.Format(ApiConstants.ArticleEndpoints.Delete, id);
				var response = await client.DeleteAsync(url);

				if (response.IsSuccessStatusCode)
					TempData["SuccessMessage"] = "Article deleted successfully";
				else
					TempData["ErrorMessage"] = "Failed to delete article";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = $"Error: {ex.Message}";
			}

			return RedirectToAction("Index", "Users");
		}
	}
}
