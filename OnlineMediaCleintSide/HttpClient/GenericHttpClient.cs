using System.Text;
using System.Text.Json;

namespace OnlineMediaCleintSide.Api
{
	public class GenericHttpClient
	{
		private readonly HttpClient _httpClient;

		public GenericHttpClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<T?> GetAsync<T>(string endpoint)
		{
			try
			{
				var response = await _httpClient.GetAsync(endpoint);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
			catch (Exception)
			{
				return default;
			}
		}

		public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
		{
			try
			{
				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync(endpoint, content);
				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
			catch (Exception)
			{
				return default;
			}
		}

		public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
		{
			try
			{
				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync(endpoint, content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
		{
			try
			{
				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await _httpClient.PutAsync(endpoint, content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> DeleteAsync(string endpoint)
		{
			try
			{
				var response = await _httpClient.DeleteAsync(endpoint);
				return response.IsSuccessStatusCode;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content)
		{
			try
			{
				var response = await _httpClient.PostAsync(endpoint, content);
				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<TResponse>(responseContent, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			}
			catch (Exception)
			{
				return default;
			}
		}
	}
}
