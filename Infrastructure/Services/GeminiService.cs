using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    /// <summary>
    /// Calls Google Gemini API to summarize extracted document text.
    /// Configure "AI:ApiKey" and "AI:Model" in appsettings or user-secrets.
    /// </summary>
    public class GeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(string Summary, string ResponseJson)> SummarizeAsync(string extractedText, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["AI:ApiKey"]
                ?? throw new InvalidOperationException("AI:ApiKey is not configured.");

            var model = _configuration["AI:Model"] ?? "gemini-2.5-flash";

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var truncatedText = extractedText.Length > 12000 ? extractedText[..12000] : extractedText;

            var requestBody = new
            {
                contents = new object[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = $"Summarize the following educational document concisely for students:\n\n{truncatedText}" }
                        }
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseJson);
            var summary = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return (summary, responseJson);
        }
    }
}