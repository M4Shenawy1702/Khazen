namespace Khazen.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class NotFoundException<T> : NotFoundException
    {
        public NotFoundException(object key)
            : base($"{typeof(T).Name} with id '{key}' was not found.") { }

        public NotFoundException(string message)
            : base($"{typeof(T).Name}: {message}") { }
    }
}
