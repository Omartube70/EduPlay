namespace Application.Exceptions
{
    public class EmailAlreadyExistsException : AppException
    {
        public EmailAlreadyExistsException(string email)
            : base($"An account with email '{email}' already exists.") { }
    }
}