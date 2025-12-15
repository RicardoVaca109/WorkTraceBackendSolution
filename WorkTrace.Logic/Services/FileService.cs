using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WorkTrace.Application.Configurations;
using WorkTrace.Application.Services;

namespace WorkTrace.Logic.Services
{
    public class FileService : IFileService
    {
        private readonly FileStorageSettings _settings;

        public FileService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var directory = Path.Combine(_settings.BasePath, folder);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"{folder}/{fileName}";
        }
    }
}