using System.Security.Cryptography;
using System.Text;

namespace SharedModule.Extensions;

public static class PasswordEncryption
{
    public static string Encrypt(this string toEncrypt, bool useHashing)
    {
        if (string.IsNullOrEmpty(toEncrypt))
            return string.Empty;

        byte[] keyArray;
        byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

        // System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
        // Get the key from config file
        //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
        string key = "SelahElTelmeez";
        if (useHashing)
        {
            MD5 hashmd5 = MD5.Create();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
        }
        else
            keyArray = Encoding.UTF8.GetBytes(key);

        TripleDES tdes = TripleDES.Create();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    public static string Decrypt(this string cipherString, bool useHashing)
    {
        if (string.IsNullOrEmpty(cipherString))
            return string.Empty;

        byte[] keyArray;
        byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
        //Get your key from config file to open the lock!
        //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
        string key = "SelahElTelmeez";

        if (useHashing)
        {
            MD5 hashmd5 = MD5.Create();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
        }
        else
            keyArray = Encoding.UTF8.GetBytes(key);

        TripleDES tdes = TripleDES.Create();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        tdes.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

}
