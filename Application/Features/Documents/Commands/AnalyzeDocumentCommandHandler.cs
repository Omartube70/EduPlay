using Application.Exceptions;
using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using System.Text.Json;

namespace Application.Features.Documents.Commands
{
    public class AnalyzeDocumentCommandHandler : IRequestHandler<AnalyzeDocumentCommand, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentAnalysisRepository _analysisRepository;
        private readonly IKeyConceptRepository _keyConceptRepository;
        private readonly ISampleQuestionRepository _sampleQuestionRepository;
        private readonly ITextExtractionService _textExtractionService;
        private readonly IAIService _aiService;
        private readonly IMapper _mapper;

        public AnalyzeDocumentCommandHandler(
            IDocumentRepository documentRepository,
            IDocumentAnalysisRepository analysisRepository,
            IKeyConceptRepository keyConceptRepository,
            ISampleQuestionRepository sampleQuestionRepository,
            ITextExtractionService textExtractionService,
            IAIService aiService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _analysisRepository = analysisRepository;
            _keyConceptRepository = keyConceptRepository;
            _sampleQuestionRepository = sampleQuestionRepository;
            _textExtractionService = textExtractionService;
            _aiService = aiService;
            _mapper = mapper;
        }

        public async Task<DocumentDto> Handle(AnalyzeDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetDocumentWithAnalysisAsync(request.DocumentId)
                ?? throw new DocumentNotFoundException(request.DocumentId);

            if (!request.IsAdmin && document.UserId != request.CurrentUserId)
                throw new ForbiddenAccessException("You don't have permission to analyze this document.");

            document.MarkAsProcessing();
            await _documentRepository.UpdateDocumentAsync(document);

            try
            {
                var extractedText = await _textExtractionService.ExtractTextAsync(document.FilePath);
                var (structuredJson, rawResponseJson) = await _aiService.SummarizeAsync(extractedText, cancellationToken);

                // Parse AI response
                var aiData = ParseAiResponse(structuredJson);

                var existingAnalysis = await _analysisRepository.GetByDocumentIdAsync(document.DocumentID);

                int analysisId;

                if (existingAnalysis == null)
                {
                    var analysis = DocumentAnalysis.Create(
                        extractedText,
                        aiData.AiSummary,
                        rawResponseJson,
                        document.DocumentID);

                    await _analysisRepository.AddAsync(analysis);
                    analysisId = analysis.DocumentAnalysisID;
                }
                else
                {
                    existingAnalysis.UpdateSummary(aiData.AiSummary, rawResponseJson);
                    await _analysisRepository.UpdateAsync(existingAnalysis);
                    analysisId = existingAnalysis.DocumentAnalysisID;

                    // Clear old data before re-saving
                    await _keyConceptRepository.DeleteByAnalysisIdAsync(analysisId);
                    await _sampleQuestionRepository.DeleteByAnalysisIdAsync(analysisId);
                }

                // Save KeyConcepts
                foreach (var kc in aiData.KeyConcepts)
                {
                    var keyConcept = KeyConcept.Create(kc.Title, kc.Description, analysisId);
                    await _keyConceptRepository.AddAsync(keyConcept);
                }

                // Save SampleQuestions + Choices
                foreach (var sq in aiData.SampleQuestions)
                {
                    var type = MapQuestionType(sq.Type);
                    var difficulty = MapDifficulty(sq.Difficulty);

                    var question = SampleQuestion.Create(
                        sq.Question,
                        type,
                        difficulty,
                        sq.CorrectAnswer,
                        sq.AnswerIndex,
                        analysisId);

                    await _sampleQuestionRepository.AddAsync(question);

                    if (type == QuestionType.MultipleChoice && sq.Choices.Count > 0)
                    {
                        await _sampleQuestionRepository.AddChoicesAsync(
                            question.SampleQuestionID,
                            sq.Choices);
                    }
                }

                document.MarkAsCompleted();
            }
            catch
            {
                document.MarkAsFailed();
                await _documentRepository.UpdateDocumentAsync(document);
                throw;
            }

            await _documentRepository.UpdateDocumentAsync(document);

            var updatedDocument = await _documentRepository.GetDocumentWithAnalysisAsync(document.DocumentID);
            return _mapper.Map<DocumentDto>(updatedDocument);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static AiResponseData ParseAiResponse(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var summary = root.TryGetProperty("aiSummary", out var s)
                    ? s.GetString() ?? string.Empty
                    : string.Empty;

                var keyConcepts = new List<AiKeyConcept>();
                if (root.TryGetProperty("keyConcepts", out var kcs) && kcs.ValueKind == JsonValueKind.Array)
                {
                    foreach (var kc in kcs.EnumerateArray())
                    {
                        var title = kc.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                        var desc = kc.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
                        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(desc))
                            keyConcepts.Add(new AiKeyConcept(title, desc));
                    }
                }

                var sampleQuestions = new List<AiSampleQuestion>();
                if (root.TryGetProperty("sampleQuestions", out var sqs) && sqs.ValueKind == JsonValueKind.Array)
                {
                    foreach (var sq in sqs.EnumerateArray())
                    {
                        var question = sq.TryGetProperty("question", out var q) ? q.GetString() ?? "" : "";
                        var type = sq.TryGetProperty("type", out var tp) ? tp.GetString() ?? "" : "";
                        var difficulty = sq.TryGetProperty("difficulty", out var df) ? df.GetString() ?? "medium" : "medium";
                        var correctAnswer = sq.TryGetProperty("correctAnswer", out var ca) ? ca.GetString() : null;

                        int? answerIndex = sq.TryGetProperty("answerIndex", out var ai) && ai.ValueKind == JsonValueKind.Number
                            ? ai.GetInt32()
                            : null;

                        var choices = new List<string>();
                        if (sq.TryGetProperty("choices", out var ch) && ch.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in ch.EnumerateArray())
                            {
                                var text = c.GetString();
                                if (!string.IsNullOrWhiteSpace(text))
                                    choices.Add(text);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(type))
                            sampleQuestions.Add(new AiSampleQuestion(question, type, difficulty, correctAnswer, answerIndex, choices));
                    }
                }

                return new AiResponseData(summary, keyConcepts, sampleQuestions);
            }
            catch
            {
                // AI response مش valid JSON — نرجع summary فاضي والـ collections فاضية
                return new AiResponseData(json, new List<AiKeyConcept>(), new List<AiSampleQuestion>());
            }
        }

        private static QuestionType MapQuestionType(string type) => type.ToLower() switch
        {
            "multiple-choice" or "multiplechoice" => QuestionType.MultipleChoice,
            _ => QuestionType.ShortAnswer
        };

        private static Difficulty MapDifficulty(string difficulty) => difficulty.ToLower() switch
        {
            "hard" => Difficulty.Hard,
            "medium" => Difficulty.Medium,
            _ => Difficulty.Easy
        };

        // ─── Internal record types ─────────────────────────────────────────────

        private record AiResponseData(
            string AiSummary,
            List<AiKeyConcept> KeyConcepts,
            List<AiSampleQuestion> SampleQuestions);

        private record AiKeyConcept(string Title, string Description);

        private record AiSampleQuestion(
            string Question,
            string Type,
            string Difficulty,
            string? CorrectAnswer,
            int? AnswerIndex,
            List<string> Choices);
    }
}