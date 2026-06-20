using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DocumentAnalysis
    {
        public int DocumentAnalysisID { get; private set; }
        public string ExtractedText { get; private set; }
        public string AiSummary { get; private set; }
        public string? AiResponseJson { get; private set; }
        public DateTime AnalyzedAt { get; private set; }

        // Foreign Keys
        public int DocumentId { get; private set; }

        // Navigation Properties
        public Document Document { get; private set; }

#pragma warning disable CS8618
        private DocumentAnalysis() { }
#pragma warning restore CS8618

        private DocumentAnalysis(string extractedText, string aiSummary, string? aiResponseJson , int documentId)
        {
            ExtractedText = extractedText;
            AiSummary = aiSummary;
            AiResponseJson = aiResponseJson;
            DocumentId = documentId;
            AnalyzedAt = DateTime.UtcNow;
        }

        public static DocumentAnalysis Create(string extractedText, string aiSummary , string? aiResponseJson, int documentId)
        {
            if (string.IsNullOrWhiteSpace(extractedText))
                throw new ArgumentException("Extracted text is required.", nameof(extractedText));

            if (string.IsNullOrWhiteSpace(aiSummary))
                throw new ArgumentException("AI summary is required.", nameof(aiSummary));

            if (documentId <= 0)
                throw new ArgumentException("Document ID must be valid.", nameof(documentId));

            return new DocumentAnalysis(extractedText, aiSummary, aiResponseJson, documentId);
        }

        public void UpdateSummary(string newSummary, string? newAiResponseJson = null)
        {
            if (string.IsNullOrWhiteSpace(newSummary))
                throw new ArgumentException("Summary cannot be empty.");

            AiSummary = newSummary;

            if (newAiResponseJson != null)
                AiResponseJson = newAiResponseJson;
        }
    }
}