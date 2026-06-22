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

        public GroqService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(string Summary, string ResponseJson)> SummarizeAsync(string extractedText, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["AI:ApiKey"]
                ?? throw new InvalidOperationException("AI:ApiKey is not configured.");

            var model = _configuration["AI:Model"] ?? "llama-3.3-70b-versatile";
            var baseUrl = _configuration["AI:BaseUrl"] ?? throw new InvalidOperationException("AI:BaseUrl is not configured.");

            var truncatedText = extractedText.Length > 12000 ? extractedText[..12000] : extractedText;

            var strictPromptInstruction = """
            Return exactly one JSON object (no surrounding text or fences). The object MUST follow this shape: { "documentId": integer or null, "aiSummary": string, "keyConcepts": [ { "title": string, "description": string } ], "sampleQuestions": [ { "question": string, "type": "short-answer"|"multiple-choice"|"true-false", "choices": [string] (required for multiple-choice), "answerIndex": integer (0-based, required for multiple-choice), "difficulty": "easy"|"medium"|"hard" (optional) } ], "metadata": { "sourceTextExcerpt": string (optional), "confidence": number 0.0-1.0 (optional) } }. Requirements: produce up to 8 items in keyConcepts and up to 8 in sampleQuestions; aiSummary should be 100–300 words; if you cannot produce structured keyConcepts/sampleQuestions, return empty arrays and put the full analysis text in aiSummary; strings must be JSON-safe (escape newlines); sampleQuestions types must be one of the three allowed values; for multiple-choice include choices and answerIndex (0-based); do not include any extra fields beyond this shape (extra fields allowed only under metadata); do NOT include any explanatory text, markdown, or code fences—output only the JSON object.
            """;

            var requestBody = new
            {
                model,
                messages = new object[]
                {
                    new { role = "system", content = strictPromptInstruction },
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