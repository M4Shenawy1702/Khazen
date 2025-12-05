namespace Khazen.Domain.Exceptions
{
    public class DomainException : BadRequestException
    {
        public DomainException(string message)
            : base(message)
        {
        }
        public DomainException(string message, string type)
            : base("Domain Exception for entity {type}: {message}")
        {
        }
    }
}
