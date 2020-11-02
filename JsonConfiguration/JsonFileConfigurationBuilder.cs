using System;
using System.Collections.Generic;
using System.IO;
using FileConfigurationInterface;

namespace JsonFileConfiguration
{
    public class JsonFileConfigurationBuilder:IFileConfigurationBuilder
    {
        private readonly Dictionary<string,Type>_filesTypes=new Dictionary<string, Type>();
        private readonly string _basePath = Directory.GetCurrentDirectory();
        public IFileConfigurationBuilder  AddConfigurationFile<T>(string relativeFilePath)
        {
            _filesTypes.Add(Path.Combine(_basePath, relativeFilePath),typeof(T));
            return this;
        }

        public IFileConfiguration Build()
        {
            return new JsonConfiguration(_filesTypes);
        }
    }
}
