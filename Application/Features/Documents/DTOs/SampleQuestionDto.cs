namespace Application.Features.Documents.DTOs
{
    public class SampleQuestionDto
    {
        public int SampleQuestionId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int? AnswerIndex { get; set; }
        public List<QuestionChoiceDto> Choices { get; set; } = new();
    }
}