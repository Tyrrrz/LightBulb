using System.Threading.Tasks;

namespace LightBulb.Services
{
    /// <summary>
    /// Implemented by classes that can check for program updates
    /// </summary>
    public interface IVersionCheckService
    {
        /// <summary>
        /// Checks for updates, returns if there are any
        /// </summary>
        Task<bool> GetUpdateStatusAsync();
    }
}
