using Khazen.Application.Common.Configurations;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Khazen.Application.Common.Services
{
    internal class TwilioSmsService : ISmsService
    {
        private readonly TwilioSmsConfigurations _config;

        public TwilioSmsService(IOptions<TwilioSmsConfigurations> config)
        {
            _config = config.Value;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }

        public async Task SendSmsAsync(SMSMessage message)
        {
            await MessageResource.CreateAsync(new PhoneNumber(message.To), from: new PhoneNumber(_config.PhoneNumber), body: message.Message);
        }
    }
}
