using AntiCaptchaNetCore;

namespace ConsultaMD.Services
{
    public static class AntiCaptchaClient
    {
        private static string ApiKey { get; set; }
        private static AntiCaptcha AntiCaptcha { get; set; }
        public static string Init(string apiKey)
        {
            ApiKey = apiKey;
            AntiCaptcha = new AntiCaptcha(apiKey);
            return ApiKey;
        }
        public static AntiCaptcha Get()
        {
            return AntiCaptcha;
        }
    }
}
