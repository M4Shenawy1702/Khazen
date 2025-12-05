namespace Khazen.Domain.Exceptions
{
    public class InvalidCredentialsException()
        : BadRequestException(new List<string> { "Invalid Credentials" });
}
