using System.Threading.Tasks;

namespace LightBulb.Services.Interfaces
{
    public interface IVersionCheckService
    {
        /// <summary>
        /// Checks for updates, returns if there are any
        /// </summary>
        Task<bool> GetUpdateStatusAsync();
    }
}
