namespace Khazen.Application.Common.ObjectValues
{
    public class SMSMessage(string to, string message)
    {
        public string To { get; set; } = to;
        public string Message { get; set; } = message;

    }
}
