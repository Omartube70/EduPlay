using Domain.Enums;

namespace Domain.Entities
{
    public class SampleQuestion
    {
        public int SampleQuestionID { get; private set; }
        public string Question { get; private set; }
        public QuestionType Type { get; private set; }
        public Difficulty Difficulty { get; private set; }
        public string? CorrectAnswer { get; private set; }
        public int? AnswerIndex { get; private set; }

        // Foreign Key
        public int DocumentAnalysisID { get; private set; }

        // Navigation Properties
        public DocumentAnalysis DocumentAnalysis { get; private set; } = null!;
        public ICollection<QuestionChoice> Choices { get; private set; } = new List<QuestionChoice>();

#pragma warning disable CS8618
        private SampleQuestion() { }
#pragma warning restore CS8618

        private SampleQuestion(string question, QuestionType type, Difficulty difficulty, string? correctAnswer, int? answerIndex, int documentAnalysisId)
        {
            Question = question;
            Type = type;
            Difficulty = difficulty;
            CorrectAnswer = correctAnswer; 
            AnswerIndex = answerIndex;
            DocumentAnalysisID = documentAnalysisId;
        }

        public static SampleQuestion Create(string question, QuestionType type, Difficulty difficulty, string? correctAnswer, int? answerIndex, int documentAnalysisId)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question text is required.", nameof(question));

            if (documentAnalysisId <= 0)
                throw new ArgumentException("DocumentAnalysis ID must be valid.", nameof(documentAnalysisId));

            if (type == QuestionType.MultipleChoice && answerIndex == null)
                throw new ArgumentException("AnswerIndex is required for multiple-choice questions.", nameof(answerIndex));

            return new SampleQuestion(question, type, difficulty, correctAnswer, answerIndex, documentAnalysisId);
        }

        public void Update(string question, QuestionType type, Difficulty difficulty, string? correctAnswer, int? answerIndex)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question text is required.", nameof(question));

            if (type == QuestionType.MultipleChoice && answerIndex == null)
                throw new ArgumentException("AnswerIndex is required for multiple-choice questions.", nameof(answerIndex));

            Question = question;
            Type = type;
            Difficulty = difficulty;
            CorrectAnswer = correctAnswer;
            AnswerIndex = answerIndex;
        }
    }
}