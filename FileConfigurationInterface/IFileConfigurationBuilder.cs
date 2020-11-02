namespace FileConfigurationInterface
{
    public interface IFileConfigurationBuilder
    {
        IFileConfigurationBuilder AddConfigurationFile<T>(string filePath);
       IFileConfiguration Build();
    }
}
