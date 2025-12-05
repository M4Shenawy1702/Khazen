namespace Khazen.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public BadRequestException(IEnumerable<string> errors)
            : base(errors?.FirstOrDefault() ?? $"Bad Request Exception") => Errors = errors;
        public BadRequestException(string error)
            : base(error ?? $"Bad Request Exception") => Errors = [error];
    }
}
