namespace Khazen.Application.Common.Configurations
{
    public class EmailConfigurations
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string FromName { get; set; }
    }
}
