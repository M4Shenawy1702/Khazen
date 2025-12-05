using Khazen.Application.Common.Configurations;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Khazen.Application.Common.Services
{
    internal class EmailSender(IOptions<EmailConfigurations> options)
        : IEmailSender
    {
        private readonly EmailConfigurations _emailConfigurations = options.Value;

        public async Task SendEmailAsync(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailConfigurations.FromName, _emailConfigurations.UserName));
            mimeMessage.To.AddRange(message.To);
            mimeMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder()
            {
                HtmlBody = string.Format(
                "<h2>{0}</h2><p>{1}</p>",
                    message.Subject,
                    message.Body
                    )
            };

            if (message.Attachments != null && message.Attachments.Count != 0)
            {
                byte[] filesBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        filesBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, filesBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            return mimeMessage;
        }

        private async Task SendAsync(MimeMessage emailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(
                    _emailConfigurations.SmtpServer,
                    _emailConfigurations.SmtpPort,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_emailConfigurations.UserName, _emailConfigurations.Password);
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
