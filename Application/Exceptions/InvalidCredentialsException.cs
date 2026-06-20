namespace Application.Exceptions
{
    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException()
            : base("Invalid email or password.") { }
    }
}