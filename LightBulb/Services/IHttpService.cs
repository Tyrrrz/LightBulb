using System.Threading.Tasks;

namespace LightBulb.Services
{
    /// <summary>
    /// Implemented by classes that handle HTTP requests
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Performs a GET request and returns content as a string
        /// </summary>
        Task<string> GetStringAsync(string url);
    }
}