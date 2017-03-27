using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TPAnagramFinder
{
    public class AnagramGenerator
    {
        private readonly string[] _validOneLetterWords;
        private readonly string[] _wordlist;
        private readonly int _minWordLength;
        private readonly int _maxWordsPerSentence;

        private Dictionary<string, string[]> _dictionary;
        private string _phrase;
        private string _phraseClean;

        private VectorConverter _vectorConverter;
        private Dictionary<int, Vector<byte>[]> _keyVectorsByLength;
        private Vector<byte> _phraseVector;

        public AnagramGenerator(string[] wordlist, int minWordLength = 4, int maxWordsPerSentence = 4)
        {
            _wordlist = wordlist;        
            _validOneLetterWords = new[] { "a", "i", "o" };
            _minWordLength = minWordLength;
            _maxWordsPerSentence = maxWordsPerSentence;
        }

        public void Initialize(string phrase)
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

            _vectorConverter = new VectorConverter(_phraseClean);

            var keyVectors = _dictionary.Keys
                .Select(k => _vectorConverter.CovertString(k))
                .ToArray();

            _keyVectorsByLength = Enumerable.Range(0, _phraseClean.Length + 1)
                .ToDictionary(index => index, index => keyVectors.Where(kv => kv.GetVectorComponentSum() <= index).ToArray());

            _phraseVector = _vectorConverter.CovertString(_phraseClean);
        }

        public IEnumerable<string> FindAnagrams(string phrase)
        {
            Initialize(phrase);

            return GetSentences(ConvertVectorCombinationsToKeyCombinations(GenerateVectorCombinations()));
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

            foreach (var key in _keyVectorsByLength[remainingValues.GetVectorComponentSum()])
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

        public IEnumerable<IEnumerable<string>> ConvertVectorCombinationsToKeyCombinations(IEnumerable<IEnumerable<Vector<byte>>> vectorCombinations)
        {
            var result = new ConcurrentBag<IEnumerable<string>>();
            Parallel.ForEach(vectorCombinations, vectorCombination =>
            {
                result.Add(ConvertVectorCombinationToKeyCombination(vectorCombination).ToList());
            });
            return result;
        }

        private IEnumerable<string> ConvertVectorCombinationToKeyCombination(IEnumerable<Vector<byte>> vectorCombination)
        {
            foreach (var vector in vectorCombination)
            {
                yield return _vectorConverter.GetStringValue(vector);
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
    }
}
