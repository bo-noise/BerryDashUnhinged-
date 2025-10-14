using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SensitiveInfo
{
    public static readonly string SERVER_DATABASE_PREFIX = "https://games.lncvrt.xyz/database/";
    public static readonly string BAZOOKA_MANAGER_KEY = "";
    public static readonly string BAZOOKA_MANAGER_FILE_KEY = "9b9ad7a5a824436144083c19dd071d66";

    public static byte[] EncryptRaw(string text, string key)
    {
        if (BAZOOKA_MANAGER_KEY.Trim() == string.Empty)
        {
            Debug.LogError("Failed to encrypt: Encryption/Decryption keys not present");
            return null;
        }
        try
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using MemoryStream ms = new();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(text);
            }

            return ms.ToArray();
        }
        catch
        {
            return null;
        }
    }

    public static string DecryptRaw(byte[] data, string key)
    {
        if (BAZOOKA_MANAGER_KEY.Trim() == string.Empty)
        {
            Debug.LogError("Failed to decrypt: Encryption/Decryption keys not present");
            return null;
        }
        try
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[16];
            Array.Copy(data, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using MemoryStream ms = new(data, iv.Length, data.Length - iv.Length);
            using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            return reader.ReadToEnd();
        }
        catch
        {
            return null;
        }
    }
}