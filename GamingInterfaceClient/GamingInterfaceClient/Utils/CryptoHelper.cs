using System.Reflection;

namespace GamingInterfaceClient.Utils
{
    class CryptoHelper : CrossPlatformAESEncryption.Helper.CryptoHelper
    {
        private const string SECRET_KEY = "5MOujLrJUpjIEqLg";

        public static new string Decrypt(string textToDecrypt)
        {
            MethodInfo method = typeof(CrossPlatformAESEncryption.Helper.CryptoHelper).GetMethod("Decrypt", BindingFlags.NonPublic | BindingFlags.Static);
            return (string)method.Invoke(null, new object[] { textToDecrypt, SECRET_KEY });
        }

        public static new string Encrypt(string textToEncrypt)
        {
            MethodInfo method = typeof(CrossPlatformAESEncryption.Helper.CryptoHelper).GetMethod("Encrypt", BindingFlags.NonPublic | BindingFlags.Static);
            return (string)method.Invoke(null, new object[] { textToEncrypt, SECRET_KEY });
        }
    }
}
