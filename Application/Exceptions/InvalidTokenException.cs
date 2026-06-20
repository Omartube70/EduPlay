namespace Application.Exceptions
{
    public class InvalidTokenException : AppException
    {
        public InvalidTokenException(string message)
            : base(message) { }
    }
}