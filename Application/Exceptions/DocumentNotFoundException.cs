namespace Application.Exceptions
{
    public class DocumentNotFoundException : AppException
    {
        public DocumentNotFoundException(int documentId)
            : base($"Document with ID '{documentId}' was not found.") { }
    }
}