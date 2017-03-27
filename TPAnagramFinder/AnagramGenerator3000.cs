using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TPAnagramFinder
{
    public class AnagramGenerator3000
    {
        private readonly string[] _validOneLetterWords;
        private readonly string[] _wordlist;
        private readonly int _minWordLength;
        private readonly int _maxWordsPerSentence;

        private Dictionary<string, string[]> _dictionary;
        private Dictionary<int, string[]> _keysByLength;
        private string _phrase;
        private string _phraseClean;

        private VectorConverter _vectorConverter;
        private Vector<byte>[] _keyVectors;
        private Vector<byte> _phraseVector;

        public AnagramGenerator3000(string[] wordlist, int minWordLength = 4, int maxWordsPerSentence = 4)
        {
            _wordlist = wordlist;        
            _validOneLetterWords = new[] { "a", "i", "o" };
            _minWordLength = minWordLength;
            _maxWordsPerSentence = maxWordsPerSentence;
        }

        // TODO temp for testing
        public void BuildDictionary(string phrase)
        {
            _phrase = phrase;
            _phraseClean = string.Join("", phrase.Trim().ToLower().Replace(" ", "").OrderBy(_ => _));

            _dictionary = _wordlist
                .Where(w => w.Length >= _minWordLength)
                .Where(w => w.IsSubSet(_phraseClean))
                .Where(w => w.Length != 1 || _validOneLetterWords.Contains(w))
                .Select(w => w.Trim())
                .Select(w => w.ToLower())
                .Distinct()
                .GroupBy(w => string.Join("", w.Replace("'", "").OrderBy(_ => _)))
                .ToDictionary(g => g.Key, g => g.ToArray());

            _keysByLength = Enumerable.Range(0, _phraseClean.Length + 1)
                .ToDictionary(index => index, index => _dictionary.Keys.Where(k => k.Length <= index).ToArray());

            _vectorConverter = new VectorConverter(_phraseClean);

            _keyVectors = _dictionary.Keys
                .Select(k => _vectorConverter.CovertString(k))
                .ToArray();

            _phraseVector = _vectorConverter.CovertString(_phraseClean);
        }

        public IEnumerable<string> FindAnagrams(string phrase)
        {
            var letterInventory = string.Join("", phrase.Trim().ToLower().OrderBy(_ => _));

            BuildDictionary(letterInventory);

            return Enumerable.Empty<string>();
        }

        public IEnumerable<IEnumerable<Vector<byte>>> GenerateVectorCombinations()
        {
            return GenerateVectorCombinations(new List<Vector<byte>>(), _phraseVector);
        }

        private IEnumerable<IEnumerable<Vector<byte>>> GenerateVectorCombinations(List<Vector<byte>> currentCombination, Vector<byte> remainingValues)
        {
            if (Vector.EqualsAll(remainingValues, Vector<byte>.Zero))
            {
                yield return currentCombination.ToList();
                yield break;
            }

            if (currentCombination.Count >= _maxWordsPerSentence)
            {
                yield break;
            }

            foreach(var key in _keyVectors)
            {
                if (Vector.GreaterThanOrEqualAll(remainingValues, key))
                {
                    currentCombination.Add(key);
                    foreach (var otherKeys in GenerateVectorCombinations(currentCombination, Vector.Subtract(remainingValues, key)))
                    {
                        yield return otherKeys;
                    }
                    currentCombination.Remove(key);
                }
            }
        }

        public IEnumerable<IEnumerable<string>> GetKeyCombinations()
        {
            return GetKeyCombinations(new List<string>(), _phraseClean);
        }

        private IEnumerable<IEnumerable<string>> GetKeyCombinations(List<string> currentCombination, string letterInventory)
        {
            if (letterInventory.Length == 0)
            {
                yield return currentCombination.ToList();
                yield break;
            }

            if(currentCombination.Count >= _maxWordsPerSentence)
            {
                yield break;
            }

            foreach (var key in _keysByLength[letterInventory.Length])
            {
                if (key.All(k => letterInventory.Contains(k)))
                {
                    currentCombination.Add(key);
                    foreach (var otherKeys in GetKeyCombinations(currentCombination, SubtractKey(letterInventory, key)))
                    {
                        yield return otherKeys;
                    }
                    currentCombination.Remove(key);
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
