namespace Application.Exceptions
{
    public class ForbiddenAccessException : AppException
    {
        public ForbiddenAccessException()
            : base("You don't have permission to perform this action.") { }

        public ForbiddenAccessException(string message)
            : base(message) { }
    }
}