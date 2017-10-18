/*
 * Copyright (c) 2015-2017, Jillian England
 * License: GPL 3
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Cryptography.AesCrypt
{
    public class AesCryptBytes : ICryptBytes
    {
        public AesCryptBytes()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="keyBase64"></param>
        public AesCryptBytes(string keyBase64) : this()
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
        /// <param name="plainBytes"></param>
        /// <returns></returns>
        public byte[] Encrypt(string keyBase64, byte[] plainBytes)
        {
            Aes.GenerateIV();
            Aes.Key = Convert.FromBase64String(keyBase64);
            return EncryptWithIv(Aes, plainBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="plainBytes"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainBytes)
        {
            Aes.GenerateIV();
            return EncryptWithIv(Aes, plainBytes);
        }

        protected byte[] EncryptWithIv(SymmetricAlgorithm aes, byte[] plainBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(Aes.IV, 0, Aes.IV.Length);

                using (var encryptor = aes.CreateEncryptor())
                using (var cryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var compressStream = new DeflateStream(cryptStream, CompressionMode.Compress))
                {
                    compressStream.Write(plainBytes, 0, plainBytes.Length);
                }

                return memoryStream.ToArray();
            }
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// </summary>
        /// <param name="keyBase64"></param>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(string keyBase64, byte[] cipherBytes)
        {
            Aes.Key = Convert.FromBase64String(keyBase64);
            return Decrypt(cipherBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] cipherBytes)
        {
            try
            {
                return DecryptToBytes(Aes, cipherBytes);
            }
            catch (CryptographicException error)
            {
                if (IsAesKeyProvided) throw;

                throw new CryptographicException("Decryption key not set. Default key can not decrypt this text",
                    error.InnerException);
            }
        }

        private static byte[] DecryptToBytes(SymmetricAlgorithm aes, byte[] cipherBytes)
        {
            var iv = new byte[16];
            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);

            var buffer = new List<byte>();
            try
            {
                using (var memoryStream = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                using (var cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var compressStream = new DeflateStream(cryptStream, CompressionMode.Decompress))
                {
                    int item;
                    while ((item = compressStream.ReadByte()) != -1)
                        buffer.Add((byte) item);
                }
            }
            catch (CryptographicException error)
            {
                throw new CryptographicException("Decryption key can not decrypt this text", error);
            }

            return buffer.ToArray();
        }

        #endregion
    }
}