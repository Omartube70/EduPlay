namespace Application.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(int userId)
            : base($"User with ID '{userId}' was not found.") { }
    }
}