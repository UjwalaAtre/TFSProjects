using System.Diagnostics;
using System;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;



namespace gloUpdates
{
    public static class clsEncryption
    {
        // Use DES CryptoService with Private key pair
        static byte[] key = new byte[] { }; // we are going to pass in the key portion in our method calls
        static byte[] IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static string DecryptFromBase64String(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
            // Note: The DES CryptoService only accepts certain key byte lengths
            // We are going to make things easy by insisting on an 8 byte legal key length

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                // we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                // now decrypt the regular string
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string EncryptToBase64String(string stringToEncrypt, string SEncryptionKey)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                // convert our input string to a byte array
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                //now encrypt the bytearray
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                // now return the byte array as a "safe for XMLDOM" Base64 String
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
