using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;

internal class MockSmsService : ISmsService
{
    public Task SendSmsAsync(SMSMessage message)
    {
        Console.WriteLine($"[MOCK SMS] To: {string.Join(",", message.To)} | Message: {message.Message}");
        return Task.CompletedTask;
    }
}
