using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    /// <summary>
    /// Calls an AI provider (e.g. OpenAI Chat Completions) to summarize extracted document text.
    /// Configure "AI:ApiKey", "AI:Endpoint" and "AI:Model" in appsettings.
    /// </summary>
    public class OpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(string Summary, string ResponseJson)> SummarizeAsync(string extractedText, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["AI:ApiKey"];
            var endpoint = _configuration["AI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";
            var model = _configuration["AI:Model"] ?? "gpt-4o-mini";

            var truncatedText = extractedText.Length > 12000 ? extractedText[..12000] : extractedText;

            var requestBody = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = "You are an assistant that summarizes educational documents concisely for students." },
                    new { role = "user", content = $"Summarize the following document:\n\n{truncatedText}" }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseJson);
            var summary = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return (summary, responseJson);
        }
    }
}