namespace Application.Features.Documents.DTOs
{
    public class QuestionChoiceDto
    {
        public int QuestionChoiceId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}