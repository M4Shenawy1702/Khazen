using Khazen.Application.Common.ObjectValues;

namespace Khazen.Application.Common.Interfaces
{
    internal interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
