using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPAnagramFinder
{
    public class AnagramGenerator3000
    {
        private const int MaxWords = 4;

        private readonly string[] _validOneLetterWords;
        private readonly string[] _wordlist;
        private readonly int _minWordLength;

        private Dictionary<string, string[]> _dictionary;
        private Dictionary<int, string[]> _keysByLength;

        public AnagramGenerator3000(string[] wordlist, int minWordLength = 4)
        {
            _wordlist = wordlist;        
            _validOneLetterWords = new[] { "a", "i", "o" };
            _minWordLength = minWordLength;
        }

        // TODO temp for testing
        public void BuildDictionary(string letterInventory)
        {
            _dictionary = _wordlist
                .Where(w => w.Length >= _minWordLength)
                .Where(w => w.IsSubSet(letterInventory))
                .Where(w => w.Length != 1 || _validOneLetterWords.Contains(w))
                .Select(w => w.Trim())
                .Select(w => w.ToLower())
                .Distinct()
                .GroupBy(w => string.Join("", w.Replace("'", "").OrderBy(_ => _)))
                .ToDictionary(g => g.Key, g => g.ToArray());

            _keysByLength = Enumerable.Range(0, letterInventory.Length + 1)
                .ToDictionary(index => index, index => _dictionary.Keys.Where(k => k.Length <= index).ToArray());
        }

        public string BuildLetterInventory(string phrase)
        {
            return phrase
                .Trim()
                .ToLower()
                .Replace(" ", "");
        }

        public IEnumerable<string> FindAnagrams(string phrase)
        {
            var letterInventory = string.Join("", phrase.Trim().ToLower().OrderBy(_ => _));

            BuildDictionary(letterInventory);

            return Enumerable.Empty<string>();
        }


        //public IEnumerable<IEnumerable<string>> GetKeyCombinations(string letterInventory, List<string> currentKeys)
        //{
        //    if (string.IsNullOrEmpty(letterInventory))
        //    {
        //        yield return currentKeys;
        //        yield break;
        //    }

        //    if (currentKeys.Count == MaxWords)
        //    {
        //        yield return Enumerable.Empty<string>();
        //    }

        //    foreach (var key in _keysByLength[letterInventory.Length])
        //    {
        //        if (letterInventory.IndexOf(key) >= 0)
        //        {
        //            currentKeys.Add(key);
        //            foreach (var combination in GetKeyCombinations(SubtractLetters(letterInventory, key), currentKeys))
        //            {
        //                yield return combination;
        //            }
        //            currentKeys.Remove(key);
        //        }
        //    }
        //}

        public IEnumerable<IEnumerable<string>> GetKeyCombinations(string letterInventory)
        {
            if (letterInventory.Length == 0)
                yield return Enumerable.Empty<string>();

            foreach (var key in _keysByLength[letterInventory.Length])
            {
                if (key.All(k => letterInventory.Contains(k)))
                {
                    foreach (var otherKeys in GetKeyCombinations(SubtractKey(letterInventory, key)))
                    {
                        yield return otherKeys.Prepend(key);
                    }
                }
            }
        }

        public IEnumerable<string> GetSentences(IEnumerable<IEnumerable<string>> keyCombinations)
        {
            var results = new ConcurrentBag<string>();
            Parallel.ForEach(keyCombinations, (combination) =>
            {
                foreach (var sentence in GetSentenceByKeys(combination))
                    results.Add(sentence);
            });
            return results;
        }

        public IEnumerable<string> GetSentenceByKeys(IEnumerable<string> keys)
        {
            if (!keys.Any())
            {
                yield return string.Empty;
                yield break;
            }

            var key = keys.First();
            foreach(var word in _dictionary[key])
            {
                foreach(var restOfWords in GetSentenceByKeys(keys.Skip(1)))
                {
                    if (string.IsNullOrEmpty(restOfWords))
                    {
                        yield return word;
                    }
                    else
                    {
                        yield return $"{word} {restOfWords}";
                    }
                }
            }
        }

        private string SubtractKey(string letterInventory, string key)
        {
            var newInventory = letterInventory.ToList();
            foreach(var letter in key)
            {
                newInventory.Remove(letter);
            }
            return string.Join("", newInventory);
        }

        private string SubtractLetters(string src, string letters)
        {
            var remaining = src.ToList();
            foreach(var letter in letters)
            {
                remaining.Remove(letter);
            }
            return string.Join("", remaining);
        }
    }
}
