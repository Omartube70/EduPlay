using Domain.Enums;

namespace Domain.Entities
{
    public class Document
    {
        public int DocumentID { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public long FileSizeInBytes { get; private set; }
        public ContentType ContentType { get; private set; }
        public ProcessingStatus ProcessingStatus { get; private set; }
        public DateTime UploadedAt { get; private set; }

        // Foreign Key
        public int UserId { get; private set; }

        // Navigation Properties
        public User User { get; private set; } = null!;
        public DocumentAnalysis? DocumentAnalysis { get; private set; }

#pragma warning disable CS8618
        private Document() { }
#pragma warning restore CS8618

        private Document(string fileName, string filePath, long fileSizeInBytes, ContentType contentType, int userId)
        {
            FileName = fileName;
            FilePath = filePath;
            FileSizeInBytes = fileSizeInBytes;
            ContentType = contentType;
            UserId = userId;
            ProcessingStatus = ProcessingStatus.Pending;
            UploadedAt = DateTime.UtcNow;
        }

        public static Document Create(string fileName, string filePath, long fileSizeInBytes, ContentType contentType, int userId)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required.", nameof(fileName));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", nameof(filePath));

            if (fileSizeInBytes <= 0)
                throw new ArgumentException("File size must be greater than zero.", nameof(fileSizeInBytes));

            if (userId <= 0)
                throw new ArgumentException("User ID must be valid.", nameof(userId));

            return new Document(fileName, filePath, fileSizeInBytes, contentType, userId);
        }

        public void MarkAsProcessing()
        {
            if (ProcessingStatus != ProcessingStatus.Pending)
                throw new InvalidOperationException("Only pending documents can be marked as processing.");

            ProcessingStatus = ProcessingStatus.Processing;
        }

        public void MarkAsCompleted()
        {
            if (ProcessingStatus != ProcessingStatus.Processing)
                throw new InvalidOperationException("Only processing documents can be marked as completed.");

            ProcessingStatus = ProcessingStatus.Completed;
        }

        public void MarkAsFailed()
        {
            ProcessingStatus = ProcessingStatus.Failed;
        }

        public void ResetToPending()
        {
            ProcessingStatus = ProcessingStatus.Pending;
        }
    }
}