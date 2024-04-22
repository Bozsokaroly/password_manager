using Android.App;
using Android.Content;
using Android.Icu.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace password_manager
{
    internal class User
    {
        private static string name_;
        public static string name { get { return name_; } set { name_ = value; } }
        private static string token_;
        public static string token { get { return token_; } set { token_ = value; } }

        private static string email_;
        public static string email { get { return email_; } set { email_ = value; } }

        private static bool tripleDOS_;
        public static bool tripleDOS { get { return tripleDOS_; } set { tripleDOS_ = value; } }

        private static byte[] salt_;
        public static byte[] salt { get { return salt_;} set { salt_ = value; } }

        private static string lang_;
        public static string lang { get { return lang_; } set { lang_ = value; } }

        public delegate string EncryptionMethod(string text, byte[] key, byte[] salt);
        public delegate string DecryptionMethod(string text, byte[] key, byte[] salt);
        public static EncryptionMethod CurrentEncryptionMethod { get; set; }
        public static DecryptionMethod CurrentDecryptionMethod { get; set; }

        public static bool Login(JObject User)
        {
            try
            {
                var sharedPreferences = Application.Context.GetSharedPreferences("User", FileCreationMode.Private);
                var editor = sharedPreferences.Edit();
                editor.PutString("name", (string)User.GetValue("name"));
                editor.PutString("email", (string)User.GetValue("email"));
                editor.PutString("token", (string)User.GetValue("token"));
                editor.PutBoolean("encrypt", (int)User.GetValue("encrypt") == 0 ? false : true);
                editor.PutString("salt", (string)User.GetValue("salt"));
                editor.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void SetData()
        {
            try
            {
                var sharedPreferences = Application.Context.GetSharedPreferences("User", FileCreationMode.Private);
                name_ = sharedPreferences.GetString("name", "");
                email_ = sharedPreferences.GetString("email", "");
                token_ = sharedPreferences.GetString("token", "");
                tripleDOS_ = sharedPreferences.GetBoolean("encrypt", false);
                SetEncryptDecryptMethod(tripleDOS_);
                string salthex = sharedPreferences.GetString("salt", "");
                salt_ = Enumerable.Range(0, salthex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(salthex.Substring(x, 2), 16))
                     .ToArray();
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public static void SetEncryptDecryptMethod(bool tripleDOS)
        {
            CurrentEncryptionMethod = tripleDOS ?
                new EncryptionMethod(TripleDESEncryption.EncryptStringToBytes_TripleDES) :
                new EncryptionMethod(AES.EncryptStringToBytes_Aes);
            CurrentDecryptionMethod = tripleDOS ?
                new DecryptionMethod(TripleDESEncryption.DecryptStringFromBytes_TripleDES) :
                new DecryptionMethod(AES.DecryptStringFromBytes_Aes);
        }
        public static void SetLoginState(bool state)
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("Login", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutBoolean("IsLoggedIn", state);
            editor.Commit();
        }
        public static void SetMasterPasswordState(bool state)
        {
            var sharedPreferences = Application.Context.GetSharedPreferences("MasterPassword", FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutBoolean("IsPasswordSet", state);
            editor.Commit();
        }
        public static bool Logout()
        {
            try
            {
                name_ = String.Empty;
                email_ = String.Empty;
                token_ = String.Empty;
                tripleDOS_ = false;
                SetLoginState(false);
                SetMasterPasswordState(false);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}