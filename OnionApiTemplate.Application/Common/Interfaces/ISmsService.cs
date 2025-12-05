using Khazen.Application.Common.ObjectValues;

namespace Khazen.Application.Common.Interfaces
{
    public interface ISmsService
    {
        Task SendSmsAsync(SMSMessage message);
    }
}
