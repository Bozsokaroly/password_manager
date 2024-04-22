using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace password_manager
{
    internal static class CryptoHelper
    {
        private static byte[] key_;
        public static byte[] key { get { return key_; } set { key_ = value; } }
        public static byte[] GenerateKey(string password, byte[] salt)
        {
            try
            {
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    return deriveBytes.GetBytes((User.tripleDOS == true ? 192 : 256) / 8 );
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public static async Task SaveKeyAsync(byte[] key)
        {
            await SecureStorage.SetAsync("encryptedData", ByteArrayToBase64String(key));
        }
        public static void SetKey()
        {
            key_ = Base64StringToByteArray(SecureStorage.GetAsync("encryptedData").Result);
        }
        public static bool CheckKey(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
           byte[] key = GenerateKey(password, salt);
           if(key.SequenceEqual(key_))
            {
             return true;
            }
            return false;
        }
        public static bool Verify(string verify , byte[] key)
        {
            try
            {
                if (User.CurrentDecryptionMethod.Invoke(verify, key, User.salt) == User.name)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public static void Logout()
        {
            key_ = null;
            SecureStorage.RemoveAll();
        }
        public static byte[] Base64StringToByteArray(string base64)
        {
            return Convert.FromBase64String(base64);
        }
        public static string ByteArrayToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
       
    }
}