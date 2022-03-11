using System.Security.Cryptography;

namespace IdentityInfrastructure.Utilities
{
    public class UtilityGenerator
    {
        public static int GetOTP(int digits)
        {
            if (digits < 3)
                return Random.Shared.Next(10, 99);
            else
                return Random.Shared.Next(MultiplyNTimes(digits), MultiplyNTimes(digits + 1) - 1);
        }
        public static string Get8UniqueDigits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }
        private static int MultiplyNTimes(int n)
        {
            if (n == 1)
                return 1;
            else
                return 10 * MultiplyNTimes(n - 1);
        }
    }
}
