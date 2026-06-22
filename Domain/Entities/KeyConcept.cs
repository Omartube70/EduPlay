namespace Domain.Entities
{
    public class KeyConcept
    {
        public int KeyConceptID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public int DocumentAnalysisID { get; private set; }

        public DocumentAnalysis DocumentAnalysis { get; private set; } = null!;

#pragma warning disable CS8618
        private KeyConcept() { }
#pragma warning restore CS8618

        private KeyConcept(string title, string description, int documentAnalysisId)
        {
            Title = title;
            Description = description;
            DocumentAnalysisID = documentAnalysisId;
        }

        public static KeyConcept Create(string title, string description, int documentAnalysisId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            if (documentAnalysisId <= 0)
                throw new ArgumentException("DocumentAnalysis ID must be valid.", nameof(documentAnalysisId));

            return new KeyConcept(title, description, documentAnalysisId);
        }

        public void Update(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            Title = title;
            Description = description;
        }
    }
}