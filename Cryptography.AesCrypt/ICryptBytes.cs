namespace Cryptography.AesCrypt
{
    internal interface ICryptBytes
    {
        byte[] Encrypt(string keyBase64, byte[] plainBytes);
        byte[] Encrypt(byte[] plainBytes);
        byte[] Decrypt(string keyBase64, byte[] cipherBytes);
        byte[] Decrypt(byte[] cipherBytes);
    }
}