namespace ConsultaMD.Services
{
    public static class AntiCaptchaClient
    {
        private static string ApiKey { get; set; }
        public static string Init(string apiKey)
        {
            ApiKey = apiKey;
            return ApiKey;
        }
        public static string Get()
        {
            return ApiKey;
        }
    }
}
