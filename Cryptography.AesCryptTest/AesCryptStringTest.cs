/*
 * Copyright (c) 2015, Jillian England
 * License: GPL 4
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Cryptography.AesCrypt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.DataGeneration;
using static System.Console;

namespace Cryptography.AesCryptTest
{
    [TestClass]
    public class AesCryptStringTest
    {
        private readonly RandomWords _randomWords = new RandomWords();

        [TestMethod]
        public void CryptConstructor()
        {
            // Verify that setting a key in the constructor works correctly.
            var crypto = new AesCryptString();

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
            var crypto = new AesCryptString();

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));
            //"The rain in spain stays mainly on the plain. Where is the rain, On the plain, on the plain. Where is that rudy plain?  In Spain, In Spain! Oh bother ... Forty Nine and eight tenths.";
            const string key = "Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=";
            var cipher = crypto.Encrypt(key, plain);

            WriteLine(cipher);

            WriteLine($"lengthDelta = {plain.Length - cipher.Length}");

            var plain2 = crypto.Decrypt(key, cipher);

            WriteLine(plain2);

            Assert.AreEqual(plain, plain2);
        }

        [TestMethod]
        public void CryptSansKey()
        {
            var crypto = new AesCryptString("Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=");

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));

            var crypt = crypto.Encrypt(plain);

            var plain2 = crypto.Decrypt(crypt);

            Assert.AreEqual(plain, plain2);
        }

        [TestMethod]
        public void ForgotToInitializeKeyForDecryption()
        {
            var crypto1 = new AesCryptString();

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));

            var crypt = crypto1.Encrypt(plain);

            var plain2 = crypto1.Decrypt(crypt);

            Assert.AreEqual(plain, plain2);

            // Verify that unset decryption key provides a useful message
            var crypto2 = new AesCryptString();

            try
            {
                crypto2.Decrypt(crypt);
                Assert.Fail("Expected this decrypt to fail because we haven't set the key yet.");
            }
            catch (CryptographicException error)
            {
                Assert.IsTrue(Regex.IsMatch(error.Message, "decryption key not set", RegexOptions.IgnoreCase),
                    error.Message);
            }
        }

        [TestMethod]
        public void IncorrectKeyForDecryption()
        {
            var keyGenerator = new AesManaged();

            keyGenerator.GenerateKey();
            var key1 = Convert.ToBase64String(keyGenerator.Key);

            keyGenerator.GenerateKey();
            var key2 = Convert.ToBase64String(keyGenerator.Key);

            Assert.AreNotEqual(key1, key2);

            var plain = _randomWords.Sentence(RandomGen.Next(20, 99));

            var crypto1 = new AesCryptString(key1);
            var cipher = crypto1.Encrypt(plain);

            var plain2 = crypto1.Decrypt(cipher); // this should work.

            Assert.AreEqual(plain, plain2);
            try
            {
                crypto1.Decrypt(key2, cipher); // this should fail
                Assert.Fail("Expected this decrypt to fail because the key was not provided.");
            }
            catch (CryptographicException error)
            {
                Assert.IsTrue(Regex.IsMatch(error.Message, "can not decrypt this text", RegexOptions.IgnoreCase),
                    error.Message);
            }
        }


        [TestMethod]
        public void FindDuplicateWords()
        {
            var allWords = _randomWords.AllWords();

            // ReSharper disable once CollectionNeverQueried.Local
            var duplicateTest = new Dictionary<string, string>();
            var countDuplicate = 0;

            using (var writer = new StreamWriter("newWords.txt"))
            {
                foreach (var item in allWords)
                    try
                    {
                        duplicateTest.Add(item.ToLower(), item);
                        writer.Write("{0} ", item);
                    }
                    catch (ArgumentException)
                    {
                        WriteLine($"dup: {item.Trim()}");
                        countDuplicate++;
                    }
            }

            WriteLine($"duplicates: {countDuplicate}");
        }
    }
}