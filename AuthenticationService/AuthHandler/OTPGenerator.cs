namespace IdentityService.AuthHandler
{
    public static class OTPGenerator
    {
        public static string OneTimeOTP(int len)
        {
            string str = "0123456789";
            int n = str.Length;
            string OTP = "";

            for (int i = 1; i <= len; i++)
                OTP += (str[((int)((Random.Shared.NextDouble() * 10) % n))]);

            return OTP;
        }
    }
}
