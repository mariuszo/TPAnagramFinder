using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPAnagramFinder
{
    class TrieAnagramSolver
    {
        internal class TrieNode
        {
            private readonly char _letter;

            public Dictionary<char, TrieNode> Children { get; private set; }

            public string Word { get; set; }

            public bool EndOfWord => !string.IsNullOrEmpty(Word);

            public TrieNode(char letter)
            {
                _letter = letter;
                Children = new Dictionary<char, TrieNode>();
            }

            public TrieNode GetChild(char letter)
            {
                if (!Children.ContainsKey(letter))
                    Children[letter] = new TrieNode(letter);

                return Children[letter];
            }

            public override string ToString()
            {
                return $"Node: {_letter} | Children count: {Children.Count} | Word: {Word}";
            }
        }

        private const int MinWordLength = 4;
        private const int MaxWords = 4;

        private readonly TrieNode _root;

        private readonly Dictionary<char, int> _phraseLetters;
        private readonly int _phraseLength;
        private readonly string _phrase;

        public TrieAnagramSolver(string[] words, string phrase)
        {
            _root = new TrieNode(' ');
            _phrase = phrase;
            _phraseLetters = phrase
                .Replace(" ", "")
                .Trim()
                .ToLower()
                .GroupBy(c => c)
                .ToDictionary(c => c.Key, c => c.Count());

            _phraseLength = phrase.Length;

            words = words
                .Where(w => w.Length >= MinWordLength)
                .Where(w => w.IsSubSet(phrase))
                .Where(w => w.Length != 1 || new[] { "a", "i", "o" }.Contains(w))
                .Distinct()
                .ToArray();

            foreach (var word in words)
                AddWord(word);
        }

        public IEnumerable<string> FindAnagrams()
        {
            return FindPhrases(_phraseLetters, new StringBuilder(), _root);
        }

        private void AddWord(string word)
        {
            var current = _root;
            word = word.ToLower().Trim();

            foreach (var letter in word)
            {
                current = current.GetChild(letter);
            }

            current.Word = word;
        }

        private bool IsWordSubsetOfAvailableLetters(string word)
        {
            return !word.Except(_phrase).Any();
        }

        private IEnumerable<string> FindPhrases(Dictionary<char, int> availableLetters, StringBuilder currentPhrase, TrieNode currentNode)
        {
            if (currentNode.EndOfWord)
            {
                //if (currentPhrase.Count(c => char.IsWhiteSpace(c)) >= MaxWords - 1)
                //{
                //    yield break;
                //}

                if (currentPhrase.Length >= _phraseLength) //Good enough???
                    yield return currentPhrase.ToString();

                //if(availableLetters.Sum(l => l.Value) <= 0)
                //{
                //    yield break;
                //}

                currentPhrase.Append(" ");

                foreach (var phrase in FindPhrases(availableLetters, currentPhrase, _root))
                    yield return phrase;

                currentPhrase.Pop();
            }

            foreach (var childNode in currentNode.Children)
            {
                if(availableLetters[childNode.Key] == 0)
                    continue;

                currentPhrase.Append(childNode.Key);
                availableLetters[childNode.Key]--;

                foreach (var phrase in FindPhrases(availableLetters, currentPhrase, childNode.Value))
                    yield return phrase;

                currentPhrase.Pop();
                availableLetters[childNode.Key]++;
            }
        }
    }
}
