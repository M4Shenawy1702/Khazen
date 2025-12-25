using Khazen.Application.Common.ObjectValues;

namespace Khazen.Application.Common.Interfaces.Authentication
{
    internal interface IRecaptchaService
    {

        Task<RecaptchaResponse> VerifyAsync(string token, string email);
    }
}
