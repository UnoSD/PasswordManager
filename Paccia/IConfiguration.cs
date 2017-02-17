using System.Threading.Tasks;

namespace Paccia
{
    interface IConfiguration
    {
        Task<string> GetAsync(ConfigurationKey configurationKey);
    }
}