using Khazen.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Khazen.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadDir = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), folder);
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return $"/{folder.Replace("\\", "/")}/{fileName}";
        }
    }
}
