using System.IO;
using System.Linq;
using Utility.DataGeneration;

namespace Cryptography.AesCryptTest
{
    public class RandomWords
    {
        private readonly string[] _randomWords;

        public RandomWords()
        {
            using (var textReader = new StreamReader("words.txt"))
            {
                _randomWords = textReader
                    .ReadToEnd()
                    .Split(' ')
                    .Where(word => !string.IsNullOrEmpty(word))
                    .ToArray();
            }
        }

        public string[] Words(int numberWords)
        {
            return RandomSelect<string>.Group(_randomWords, numberWords);
        }

        public string Sentence(int numberWords)
        {
            return string.Join(" ", Words(numberWords));
        }

        public string[] AllWords()
        {
            return (string[]) _randomWords.Clone();
        }
    }
}