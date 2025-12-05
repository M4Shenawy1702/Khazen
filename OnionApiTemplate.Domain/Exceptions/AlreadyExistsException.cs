namespace Khazen.Domain.Exceptions
{
    public class AlreadyExistsException<T>
        : ConflictException
    {
        public AlreadyExistsException(string message)
            : base($"{typeof(T).Name} already exists: {message}")
        {
        }

        public AlreadyExistsException()
            : base($"{typeof(T).Name} already exists.")
        {
        }
    }
}
