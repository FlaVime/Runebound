using System;
using System.Text;

public static class CryptoUtils
{
    private static readonly string key = "57641922"; // 32-bit key for XOR encryption

    public static string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        for (int i = 0; i < plainBytes.Length; i++)
        {
            plainBytes[i] ^= keyBytes[i % keyBytes.Length];
        }

        return Convert.ToBase64String(plainBytes);
    }

    public static string Decrypt(string encryptedBase64)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            encryptedBytes[i] ^= keyBytes[i % keyBytes.Length];
        }

        return Encoding.UTF8.GetString(encryptedBytes);
    }
}
