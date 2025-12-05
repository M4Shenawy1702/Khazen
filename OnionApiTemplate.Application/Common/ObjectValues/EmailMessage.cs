using Microsoft.AspNetCore.Http;
using MimeKit;

namespace Khazen.Application.Common.ObjectValues
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IFormFileCollection? Attachments { get; set; }
        public EmailMessage(List<MailboxAddress> to, string subject, string body, IFormFileCollection attachment)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x.Name, x.Address)));
            Subject = subject;
            Body = body;
            Attachments = attachment;
        }
    }
}
