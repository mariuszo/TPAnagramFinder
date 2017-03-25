using System.Collections.Generic;
using System.Linq;

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

        private const int MinWordLength = 3;

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

            foreach (var word in words.Where(w => w.Length >= MinWordLength).Where(IsWordSubsetOfAvailableLetters))
                AddWord(word);
        }

        public IEnumerable<string> FindAnagrams()
        {
            return FindPhrases(_phraseLetters, new Stack<char>(), _root);
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

        private IEnumerable<string> FindPhrases(Dictionary<char, int> phraseLetters, Stack<char> currentPhrase, TrieNode currentNode)
        {
            if (currentNode.EndOfWord)
            {
                if (currentPhrase.Count >= _phraseLength) //Good enough
                    yield return string.Join("", currentPhrase.Reverse());

                currentPhrase.Push(' ');

                foreach (var phrase in FindPhrases(phraseLetters, currentPhrase, _root))
                    yield return phrase;

                currentPhrase.Pop();
            }

            foreach (var childNode in currentNode.Children)
            {
                if(phraseLetters[childNode.Key] == 0)
                    continue;

                currentPhrase.Push(childNode.Key);
                phraseLetters[childNode.Key]--;

                foreach (var phrase in FindPhrases(phraseLetters, currentPhrase, childNode.Value))
                    yield return phrase;

                currentPhrase.Pop();
                phraseLetters[childNode.Key]++;
            }
        }
    }
}
