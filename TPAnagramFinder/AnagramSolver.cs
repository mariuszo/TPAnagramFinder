using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPAnagramFinder
{
    public struct Occurence
    {
        public char Letter { get; private set; }
        public int Count { get; private set; }

        public Occurence(char letter, int count)
        {
            Letter = letter;
            Count = count;
        }

        public override string ToString()
        {
            return $"{Letter} | {Count}";
        }

        public override bool Equals(Object obj)
        {
            return obj is Occurence && this == (Occurence)obj;
        }

        public override int GetHashCode()
        {
            return Letter.GetHashCode() ^ Count.GetHashCode();
        }

        public static bool operator ==(Occurence x, Occurence y)
        {
            return x.Letter == y.Letter && x.Count == y.Count;
        }

        public static bool operator !=(Occurence x, Occurence y)
        {
            return !(x == y);
        }
    }

    public class AnagramSolver
    {
        private readonly HashSet<string> _dictionary;

        private Dictionary<List<Occurence>, List<string>> _dictionaryByOccurences;

        public AnagramSolver(IEnumerable<string> dictionary, string phrase)
        {
            _dictionary = new HashSet<string>(dictionary.Where(word => !word.Except(phrase).Any()));

            _dictionaryByOccurences = _dictionary
                .Where(w => w.Length > 3)
                .GroupBy(GetWordOccurences, new OccurenceListEqualityComparer())
                .ToDictionary(g => g.Key, g => g.ToList(), new OccurenceListEqualityComparer());
        }

        public List<Occurence> GetWordOccurences(string word)
        {
            return word
                .ToLower()
                .GroupBy(_ => _)
                .OrderBy(g => g.Key)
                .Select(g => new Occurence(g.Key, g.Count()))
                .ToList();
        }

        public List<Occurence> GetSentenceOccurences(string sentence)
        {
            return GetWordOccurences(sentence.Trim().Replace(" ", ""));
        }

        public IEnumerable<string> GetWordAnagrams(string word)
        {
            var result = Enumerable.Empty<string>();
            var key = GetWordOccurences(word);
            if (_dictionaryByOccurences.ContainsKey(key))
                result = _dictionaryByOccurences[key];
            return result;
        }

        public IEnumerable<string> GetWordsByOccurences(List<Occurence> occurences)
        {
            if (_dictionaryByOccurences.ContainsKey(occurences))
                return _dictionaryByOccurences[occurences];

            return Enumerable.Empty<string>();
        }

        public IEnumerable<List<Occurence>> GetOccurenceCobinations(IEnumerable<Occurence> occurences)
        {
            if (occurences == null || !occurences.Any())
            {
                yield return new List<Occurence>();
                yield break;
            }

            var entry = occurences.First();
            foreach(var currentCombination in Enumerable.Range(0, entry.Count + 1).Select(count => new Occurence(entry.Letter, count)))
            {
                var tail = occurences.Skip(1).ToList();
                foreach (var restOfCombinations in GetOccurenceCobinations(tail))
                {
                    if (currentCombination.Count == 0)
                    {
                        yield return restOfCombinations;
                    } else
                    {
                        yield return restOfCombinations.Prepend(currentCombination).ToList();
                    }
                }
            }
        }

        public IEnumerable<Occurence> SubtractOccurences(IEnumerable<Occurence> x, IEnumerable<Occurence> y)
        {
            return SubtractOccurences(x, y, new List<Occurence>()).Where(occ => occ.Count > 0).OrderBy(occ => occ.Letter);
        }

        private IEnumerable<Occurence> SubtractOccurences(IEnumerable<Occurence> x, IEnumerable<Occurence> y, IEnumerable<Occurence> acc)
        {
            if (x.Any() && !y.Any()) return acc.Concat(x);
            if (!x.Any() && y.Any()) return acc.Concat(y);
            if (!x.Any() && !y.Any()) return acc;

            var xFirst = x.First();
            var yFirst = y.First();
            if (xFirst.Letter != yFirst.Letter)
            {
                return SubtractOccurences(x.Skip(1), y, acc.Prepend(xFirst));
            } else
            {
                return SubtractOccurences(x.Skip(1), y.Skip(1), acc.Prepend(new Occurence(xFirst.Letter, xFirst.Count - yFirst.Count)));
            }
        }

        public IEnumerable<string> GetSentenceAnagrams(string sentence)
        {
            return GetOccurenceSentences(GetSentenceOccurences(sentence));
        }

        private IEnumerable<string> GetOccurenceSentences(IEnumerable<Occurence> occurences)
        {
            if(!occurences.Any())
            {
                yield return string.Empty;
                yield break;
            }

            foreach(var occurenceCombination in GetOccurenceCobinations(occurences))
            {
                foreach(var wordByOccurences in GetWordsByOccurences(occurenceCombination))
                {
                    foreach(var restOfSentence in GetOccurenceSentences(SubtractOccurences(occurences, occurenceCombination)))
                    {
                        yield return $"{restOfSentence} {wordByOccurences}";
                    }
                }
            }
        }
    }

    public class OccurenceListEqualityComparer : IEqualityComparer<List<Occurence>>
    {
        public bool Equals(List<Occurence> x, List<Occurence> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<Occurence> obj)
        {
            return obj.Aggregate(17, (acc, occ) => acc ^ occ.GetHashCode());
        }
    }
}
