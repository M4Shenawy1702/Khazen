using Microsoft.AspNetCore.Http;

namespace Khazen.Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string fileUrl);
    }
}
