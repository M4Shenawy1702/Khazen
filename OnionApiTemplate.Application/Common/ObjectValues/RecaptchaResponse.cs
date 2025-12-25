namespace Khazen.Application.Common.ObjectValues
{
    public class RecaptchaResponse
    {
        public bool Success { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
    }
}
