using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<(string FilePath, long SizeInBytes)> SaveFileAsync(IFormFile file);
        Task<(byte[] FileBytes, string ContentType)> GetFileAsync(string filePath);
        Task DeleteFileAsync(string filePath);
    }
}