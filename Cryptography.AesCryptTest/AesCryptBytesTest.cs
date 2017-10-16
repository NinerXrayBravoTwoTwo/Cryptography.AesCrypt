using System;
using System.Linq;
using System.Text;
using Cryptography.AesCrypt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.DataGeneration;
using static System.Console;

namespace Cryptography.AesCryptTest
{
    [TestClass]
    public class AesCryptBytesTest
    {
        private readonly RandomWords _randomWords = new RandomWords();

        [TestMethod]
        public void CryptConstructor()
        {
            var crypto = new AesCryptBytes();

            const string key = "Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=";

            var keybytes = Convert.FromBase64String(key);

            crypto.Aes.Key = keybytes;

            var compareBytes = crypto.Aes.Key.ToArray();

            Assert.AreEqual(keybytes.Length, compareBytes.Length);

            for (var i = 0; i < keybytes.Length; i++)
                Assert.AreEqual(keybytes[i], compareBytes[i]);
        }

        [TestMethod]
        public void CryptWithKey()
        {
            var crypto = new AesCryptBytes();

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));
            var plainBytes = Encoding.Unicode.GetBytes(plain);
            const string key = "Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=";

            var cipher = crypto.Encrypt(key, plainBytes);

            Assert.IsNotNull(cipher);

            WriteLine(crypto.Aes.IV.Length);

            Assert.IsTrue(cipher.Length > 22);

            var plain2 = crypto.Decrypt(key, cipher);

            Assert.IsNotNull(plain2);

            var plainAgain = Encoding.Unicode.GetString(plain2);

            Assert.AreEqual(plain, plainAgain);
        }

        [TestMethod]
        public void CryptSansKey()
        {
            const string keyBase64 = "Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=";

            var crypto = new AesCryptString(keyBase64);

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));

            var crypt = crypto.Encrypt(plain);

            var plain2 = crypto.Decrypt(crypt);

            Assert.AreEqual(plain, plain2);
        }
    }
}