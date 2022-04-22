using System.Security.Cryptography;

namespace TeacherInfrastructure.Utilities
{
    public static class IFormFileExtension
    {
        public static string ComputeMD5Hash(this Stream fileStream)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
