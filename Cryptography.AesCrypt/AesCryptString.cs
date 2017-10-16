/*
 * Copyright (c) 2015, Jillian England
 * License: GPL 4
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace Cryptography.AesCrypt
{
    public class AesCryptString : ICryptString
    {
        public AesCryptString()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="keyBase64"></param>
        public AesCryptString(string keyBase64) : this()
        {
            Aes.Key = Convert.FromBase64String(keyBase64);
            IsAesKeyProvided = true;
        }

        private bool IsAesKeyProvided { get; }

        /// <summary>
        /// </summary>
        public AesManaged Aes { get; } = new AesManaged();

        #region dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Aes?.Dispose();
        }

        #endregion

        #region Encrypt

        /// <summary>
        /// </summary>
        /// <param name="keyBase64"></param>
        /// <param name="plain"></param>
        /// <returns></returns>
        public string Encrypt(string keyBase64, string plain)
        {
            Aes.GenerateIV();
            Aes.Key = Convert.FromBase64String(keyBase64);
            return EncryptToBase64(Aes, plain);
        }

        /// <summary>
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public string Encrypt(string plain)
        {
            Aes.GenerateIV();
            return EncryptToBase64(Aes, plain);
        }

        protected string EncryptToBase64(SymmetricAlgorithm aes, string plain)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var encryptor = aes.CreateEncryptor())
                using (var cryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var compressStream = new DeflateStream(cryptStream, CompressionMode.Compress))
                using (var streamWriter = new StreamWriter(compressStream))
                {
                    streamWriter.Write(plain);
                }

                return $"{Convert.ToBase64String(aes.IV)}_{Convert.ToBase64String(memoryStream.ToArray())}";
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// </summary>
        /// <param name="keyBase64"></param>
        /// <param name="cipherBase64"></param>
        /// <returns></returns>
        public string Decrypt(string keyBase64, string cipherBase64)
        {
            Aes.Key = Convert.FromBase64String(keyBase64);
            return Decrypt(cipherBase64);
        }

        /// <summary>
        /// </summary>
        /// <param name="cipherbase64"></param>
        /// <returns></returns>
        public string Decrypt(string cipherbase64)
        {
            try
            {
                return DecryptToString(Aes, cipherbase64);
            }
            catch (CryptographicException error)
            {
                if (IsAesKeyProvided) throw;

                throw new CryptographicException("Decryption key not set. Default key can not decrypt this text",
                    error.InnerException);
            }
        }

        private static string DecryptToString(SymmetricAlgorithm aes, string cipherbase64)
        {
            string result;
            var cipherSplit = cipherbase64.Split('_');
            try
            {
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(cipherSplit.Last())))
                using (var decryptor = aes.CreateDecryptor(aes.Key, Convert.FromBase64String(cipherSplit.First())))
                using (var cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var compressStream = new DeflateStream(cryptStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(compressStream))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (CryptographicException error)
            {
                throw new CryptographicException("Decryption key can not decrypt this text", error);
            }

            return result;
        }

        #endregion
    }
}