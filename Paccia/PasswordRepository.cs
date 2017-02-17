using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paccia
{
    public class PasswordRepository
    {
        readonly ISerializer<IEnumerable<Secret>> _serializer;
        readonly Lazy<Task<string>> _secretsFilePath;

        internal PasswordRepository(IConfiguration configuration, ISerializer<IEnumerable<Secret>> serializer)
        {
            _serializer = serializer;
            _secretsFilePath  = new Lazy<Task<string>>(() => configuration.GetAsync(ConfigurationKey.SecretsFilePath));
        }

        public async Task<IReadOnlyCollection<Secret>> LoadPasswordsAsync()
        {
            var filePath = await _secretsFilePath;

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                Logger.Log("File not found");

                return new Secret[0];
            }
            
            using (var fileStream = fileInfo.OpenRead())
            using (var memoryStream = new MemoryStream(new byte[fileStream.Length]))
            {
                await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);

                memoryStream.Position = 0;

                var secrets = await _serializer.DeserializeAsync(memoryStream).ConfigureAwait(false);

                return secrets.ToArray();
            }
        }
        
        public async Task SavePasswordsAsync(IEnumerable<Secret> passwords)
        {
            var filePath = await _secretsFilePath;

            using (var memoryStream = new MemoryStream())
            using (var fileStream = new FileInfo(filePath).OpenWrite())
            {
                await _serializer.SerializeAsync(memoryStream, passwords).ConfigureAwait(false);

                memoryStream.Position = 0;
                   
                await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}