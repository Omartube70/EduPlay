using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    /// <summary>
    /// Calls Google Gemini API to summarize extracted document text into a strictly structured JSON.
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

            var strictPromptInstruction = """
            Return exactly one JSON object (no surrounding text or fences). The object MUST follow this shape: { "documentId": integer or null, "aiSummary": string, "keyConcepts": [ { "title": string, "description": string } ], "sampleQuestions": [ { "question": string, "type": "short-answer"|"multiple-choice"|"true-false", "choices": [string] (required for multiple-choice), "answerIndex": integer (0-based, required for multiple-choice), "difficulty": "easy"|"medium"|"hard" (optional) } ], "metadata": { "sourceTextExcerpt": string (optional), "confidence": number 0.0-1.0 (optional) } }. Requirements: produce up to 8 items in keyConcepts and up to 8 in sampleQuestions; aiSummary should be 100–300 words; if you cannot produce structured keyConcepts/sampleQuestions, return empty arrays and put the full analysis text in aiSummary; strings must be JSON-safe (escape newlines); sampleQuestions types must be one of the three allowed values; for multiple-choice include choices and answerIndex (0-based); do not include any extra fields beyond this shape (extra fields allowed only under metadata); do NOT include any explanatory text, markdown, or code fences—output only the JSON object.
            """;

            var requestBody = new
            {
                contents = new object[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = $"Analyze the following educational content and extract the structured data:\n\n{truncatedText}" }
                        }
                    }
                },
                systemInstruction = new
                {
                    parts = new object[]
                    {
                        new { text = strictPromptInstruction }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json" 
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(responseJson);

            var structuredResultJson = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return (structuredResultJson, responseJson);
        }
    }
}