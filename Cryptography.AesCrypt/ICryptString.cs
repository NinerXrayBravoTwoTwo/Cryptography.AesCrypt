namespace Cryptography.AesCrypt
{
    internal interface ICryptString
    {
        string Encrypt(string keyBase64, string plain);
        string Encrypt(string plain);
        string Decrypt(string keyBase64, string cipherBase64);
        string Decrypt(string cipherBase64);
    }
}