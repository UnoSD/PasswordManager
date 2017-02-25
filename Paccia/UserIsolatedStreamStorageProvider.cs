using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;

namespace Paccia
{
    class UserIsolatedStreamStorageProvider : IStreamStorageProvider
    {
        readonly Logger _logger;
        readonly Lazy<Task<string>> _storageFilePath;

        internal UserIsolatedStreamStorageProvider(IConfiguration configuration, ConfigurationKey filePathConfigurationKey, Logger logger)
        {
            _logger = logger;
            _storageFilePath = new Lazy<Task<string>>(() => configuration.GetAsync(filePathConfigurationKey));
        }

        public async Task<Stream> GetStorageStream(FileAccess access)
        {
            var filePath = await _storageFilePath;

            var fileInfo = new FileInfo(filePath);
            
            var store = IsolatedStorageFile.GetUserStoreForApplication();

            var fileExist = store.GetFileNames(fileInfo.Name).Any(name => name == fileInfo.Name);

            var writeRequest = (access | FileAccess.Write) == FileAccess.Write;

            if (fileExist || writeRequest)
                return new IsolatedStorageFileStream
                       (
                           filePath,
                           writeRequest ? FileMode.OpenOrCreate : FileMode.Open,
                           access,
                           store
                       );

            _logger.Log("File not found");

            return Stream.Null;
        }
    }
}