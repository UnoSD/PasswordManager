using System.Threading.Tasks;

namespace Paccia
{
    public interface IConfiguration
    {
        Task<string> GetAsync(ConfigurationKey configurationKey);
    }
}