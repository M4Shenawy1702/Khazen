using Khazen.Application.Common.Configurations;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Services;
using Microsoft.Extensions.Options;

namespace Khazen.Application.Common.Factories
{
    public class SMSFactory
    {
        public static ISmsService CreateAccount(string type, IOptions<TwilioSmsConfigurations>? twilioConfig = null)
        {
            return type switch
            {
                "Mock" => new MockSmsService(),
                "Twilio" when twilioConfig is not null => new TwilioSmsService(twilioConfig),
                _ => throw new ArgumentException("Invalid or missing SMS service configuration.")
            };
        }
    }
}
