using System;
using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    class UserFileStorageProvider : IStreamStorageProvider
    {
        readonly Logger _logger;
        readonly Lazy<Task<string>> _storageFilePath;

        public UserFileStorageProvider(IConfiguration configuration, ConfigurationKey filePathConfigurationKey, Logger logger)
        {
            _logger = logger;
            _storageFilePath = new Lazy<Task<string>>(() => configuration.GetAsync(filePathConfigurationKey));
        }

        public async Task<Stream> GetStorageStream(FileAccess access)
        {
            var filePath = await _storageFilePath;

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists || (access | FileAccess.Write) == FileAccess.Write)
                return fileInfo.Open(access == FileAccess.Read ? FileMode.Open : FileMode.OpenOrCreate, access);

            _logger.Log("File not found");

            return Stream.Null;
        }
    }
}