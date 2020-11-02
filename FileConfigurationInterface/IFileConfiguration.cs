using System.Threading.Tasks;

namespace FileConfigurationInterface
{
    public interface IFileConfiguration
    {
        Task SetConfigurationInFileAsync<T>(T dataObject);
        T GetConfig<T>() where T : class;
    }
}
