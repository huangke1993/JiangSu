using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FileConfigurationInterface;

namespace JsonFileConfiguration
{
    public class JsonConfiguration:IFileConfiguration,IDisposable
    {
        private readonly Dictionary<string, Type> _jsonFileDictionary=new Dictionary<string, Type>();
        private readonly Dictionary<Type, object> _jsonDataDictionary=new Dictionary<Type, object>();
        private readonly List<FileSystemWatcher> _fileSystemWatchers=new List<FileSystemWatcher>();
        private readonly AutoResetEvent _autoResetEvent=new AutoResetEvent(true);
        public JsonConfiguration(IEnumerable<KeyValuePair<string, Type>> files)
        {
            foreach (var file in files)
            {
                AddConfigurationFile(file.Key,file.Value);
            }
        }

        public async Task SetConfigurationInFileAsync<T>(T dataObject)
        {
            _autoResetEvent.WaitOne();
            var fileCollection= _jsonFileDictionary.Where(x => x.Value == typeof(T)).Select(x=>x.Key);
            foreach (var filePath in fileCollection)
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        await sr.WriteAsync(JsonConvert.SerializeObject(dataObject));
                    }
                }
            }
            _autoResetEvent.Set();
        }

        public void Dispose()
        {
            DisPoseFileWatcher();
            DisPoseAutoResetEvent();
        }

        private void DisPoseFileWatcher()
        {
            foreach (var fileSystemWatcher in _fileSystemWatchers)
            {
                fileSystemWatcher.Dispose();
            }
        }

        private void DisPoseAutoResetEvent()
        {
            _autoResetEvent.Dispose();
        }

        private void AddConfigurationFile(string filePath,Type type)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath == null)
                return;
            _jsonFileDictionary.Add(filePath, type);
            var fileSystemWatcher = new FileSystemWatcher(directoryPath, Path.GetFileName(filePath))
            {
                EnableRaisingEvents = true, NotifyFilter = NotifyFilters.LastWrite,
            };
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatchers.Add(fileSystemWatcher);
            ReloadConfig(filePath);
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!_jsonFileDictionary.ContainsKey(e.FullPath)) return;
            ReloadConfig(e.FullPath);
        }

        public T GetConfig<T>()
            where T : class
        {
            return _jsonDataDictionary[typeof(T)] as T;
        }

        private void ReloadConfig(string filePath)
        {
            _autoResetEvent.WaitOne();
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var sr = new StreamReader(fs))
                {
                    var jsonData = sr.ReadToEnd();
                    var data = JsonConvert.DeserializeObject(jsonData, _jsonFileDictionary[filePath]);
                    _jsonDataDictionary[_jsonFileDictionary[filePath]] = data;
                }
            }
            _autoResetEvent.Set();
        }
    }
}
