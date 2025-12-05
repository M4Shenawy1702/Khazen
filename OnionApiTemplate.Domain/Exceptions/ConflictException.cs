namespace Khazen.Domain.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }

        public ConflictException(IEnumerable<string> messages)
            : base(string.Join(", ", messages))
        {
        }
    }
}
