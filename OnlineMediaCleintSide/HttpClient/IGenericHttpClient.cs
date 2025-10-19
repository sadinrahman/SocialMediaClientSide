using System.Threading.Tasks;

namespace OnlineMediaCleintSide.Api
{
	public interface IGenericHttpClient
	{
		Task<T?> GetAsync<T>(string endpoint);
		Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
		Task<bool> PostAsync<TRequest>(string endpoint, TRequest data);
		Task<bool> PutAsync<TRequest>(string endpoint, TRequest data);
		Task<bool> DeleteAsync(string endpoint);
	}
}
