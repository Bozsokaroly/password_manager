using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace password_manager
{
    internal static class TripleDESEncryption
    {
        public static string EncryptStringToBytes_TripleDES(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                return plainText;
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (TripleDES tripleDES = TripleDES.Create())
            {
                KeySizes[] keySizes = tripleDES.LegalKeySizes;
                string keysizes = null;
                foreach (KeySizes k in keySizes)
                {
                    keysizes += k.MinSize + " " + k.MaxSize + " ";
                }
                tripleDES.Key = Key;
                tripleDES.IV = IV;
                ICryptoTransform encryptor = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptStringFromBytes_TripleDES(string cipherTextString, byte[] Key, byte[] IV)
        {
            if (string.IsNullOrEmpty(cipherTextString) || cipherTextString.Length % 4 != 0
           || cipherTextString.Contains(" ") || cipherTextString.Contains("\t") || cipherTextString.Contains("\r") || cipherTextString.Contains("\n"))
            {
                return cipherTextString;
            }

            byte[] cipherText = Convert.FromBase64String(cipherTextString);

            if (cipherText == null || cipherText.Length <= 0)
                return cipherTextString;
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (TripleDES tripleDES = TripleDES.Create())
            {
                tripleDES.Key = Key;
                tripleDES.IV = IV;
                ICryptoTransform decryptor = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}