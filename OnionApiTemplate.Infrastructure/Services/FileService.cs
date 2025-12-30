using Khazen.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Khazen.Infrastructure.Services
{
    public class FileService(ILogger<FileService> logger) : IFileService
    {
        private readonly ILogger<FileService> _logger = logger;

        public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder.TrimStart('\\', '/'));

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"/{folder.TrimStart('\\', '/').Replace("\\", "/")}/{fileName}";
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return Task.FromResult(false);

            try
            {
                var relativePath = fileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted successfully from: {Path}", fullPath);
                    return Task.FromResult(true);
                }

                _logger.LogWarning("Delete failed: File not found at: {Path}", fullPath);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting file: {Url}", fileUrl);
                return Task.FromResult(false);
            }
        }
    }
}