namespace Domain.Entities
{
    public class QuestionChoice
    {
        public int QuestionChoiceID { get; private set; }
        public string Text { get; private set; }
        public int OrderIndex { get; private set; }

        // Foreign Key
        public int SampleQuestionID { get; private set; }

        // Navigation Property
        public SampleQuestion SampleQuestion { get; private set; } = null!;

#pragma warning disable CS8618
        private QuestionChoice() { }
#pragma warning restore CS8618

        private QuestionChoice(string text, int orderIndex, int sampleQuestionId)
        {
            Text = text;
            OrderIndex = orderIndex;
            SampleQuestionID = sampleQuestionId;
        }

        public static QuestionChoice Create(string text, int orderIndex, int sampleQuestionId)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Choice text is required.", nameof(text));

            if (orderIndex < 0)
                throw new ArgumentException("OrderIndex must be non-negative.", nameof(orderIndex));

            if (sampleQuestionId <= 0)
                throw new ArgumentException("SampleQuestion ID must be valid.", nameof(sampleQuestionId));

            return new QuestionChoice(text, orderIndex, sampleQuestionId);
        }

        public void Update(string text, int orderIndex)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Choice text is required.", nameof(text));

            Text = text;
            OrderIndex = orderIndex;
        }
    }
}