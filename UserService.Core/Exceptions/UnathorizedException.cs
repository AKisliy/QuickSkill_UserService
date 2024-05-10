namespace UserService.Core.Exceptions
{
    public class UnathorizedException : Exception
    {
        public UnathorizedException()
        {
        }

        public UnathorizedException(string? message) : base(message)
        {
        }

        public UnathorizedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}