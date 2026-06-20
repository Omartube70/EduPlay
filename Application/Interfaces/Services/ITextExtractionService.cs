namespace Application.Interfaces.Services
{
    public interface ITextExtractionService
    {
        Task<string> ExtractTextAsync(string filePath);
    }
}