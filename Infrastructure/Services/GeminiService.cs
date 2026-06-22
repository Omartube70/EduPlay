using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class GroqService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _systemPrompt;

        public GroqService(HttpClient httpClient, IConfiguration configuration)
        {

            _httpClient = httpClient;
            _configuration = configuration;

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var promptPath = Path.Combine(basePath, "Prompts", "SystemPrompt.txt");

            if (File.Exists(promptPath))
            {
                _systemPrompt = File.ReadAllText(promptPath);
            }
            else
            {
                throw new FileNotFoundException("System prompt file is missing!", promptPath);
            }
        }

        public async Task<(string Summary, string ResponseJson)> SummarizeAsync(string extractedText, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["AI:ApiKey"]
                ?? throw new InvalidOperationException("AI:ApiKey is not configured.");

            var model = _configuration["AI:Model"] ?? "llama-3.3-70b-versatile";
            var baseUrl = _configuration["AI:BaseUrl"] ?? throw new InvalidOperationException("AI:BaseUrl is not configured.");

            var truncatedText = extractedText.Length > 12000 ? extractedText[..12000] : extractedText;
           
            var requestBody = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = _systemPrompt },
                    new { role = "user", content = $"Analyze the following educational content and extract the structured data:\n\n{truncatedText}" }
                },
                response_format = new { type = "json_object" },
                temperature = 0.2
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseJson);

            var structuredResultJson = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return (structuredResultJson, responseJson);
        }
    }
}