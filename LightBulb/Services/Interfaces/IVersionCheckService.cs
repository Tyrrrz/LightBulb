using System.Threading.Tasks;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Implemented by classes that can check whether the program's version is up to date
    /// </summary>
    public interface IVersionCheckService
    {
        /// <summary>
        /// Checks for updates, returns if there are any
        /// </summary>
        Task<bool> GetUpdateStatusAsync();
    }
}
