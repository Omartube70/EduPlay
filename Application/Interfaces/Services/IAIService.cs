namespace Application.Interfaces.Services
{
    public interface IAIService
    {
        Task<(string Summary, string ResponseJson)> SummarizeAsync(string extractedText, CancellationToken cancellationToken = default);
    }
}