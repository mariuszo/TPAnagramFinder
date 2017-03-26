using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPAnagramFinder
{
    class TrieAnagramSolver
    {
        internal class TrieNode
        {
            public Dictionary<char, TrieNode> Children { get; private set; }

            public bool IsTerminal { get; set; }

            public TrieNode()
            {
                Children = new Dictionary<char, TrieNode>();
            }

            public TrieNode GetChild(char letter)
            {
                if (!Children.ContainsKey(letter))
                    Children[letter] = new TrieNode();

                return Children[letter];
            }

            public override string ToString()
            {
                return $"Node: Children count: {Children.Count} | IsFinal: {IsTerminal}";
            }
        }

        private const int MinWordLength = 3;
        private const int MaxWords = 4;

        private readonly TrieNode _root;

        private readonly Dictionary<char, int> _phraseLetters;
        private readonly int _phraseLength;
        private readonly string _phrase;

        public TrieAnagramSolver(string[] words, string phrase)
        {
            _root = new TrieNode();
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

            current.IsTerminal = true;
        }

        private bool IsWordSubsetOfAvailableLetters(string word)
        {
            return !word.Except(_phrase).Any();
        }

        private readonly List<string> Anagrams = new List<string>();

        //private void FindPhrases(Dictionary<char, int> availableLetters, StringBuilder currentPhrase, TrieNode currentNode)
        //{
        //    if (currentNode.IsTerminal)
        //    {
        //        if (currentPhrase.WhiteSpaceCount() >= MaxWords - 1)
        //        {
        //            return;
        //        }

        //        if (currentPhrase.Length >= _phraseLength) //Good enough???
        //            Anagrams.Add(currentPhrase.ToString());

        //        currentPhrase.Append(" ");

        //        FindPhrases(availableLetters, currentPhrase, _root);

        //        currentPhrase.Pop();
        //    }

        //    foreach (var childNode in currentNode.Children)
        //    {
        //        if (availableLetters[childNode.Key] == 0)
        //            continue;

        //        currentPhrase.Append(childNode.Key);
        //        availableLetters[childNode.Key]--;

        //        FindPhrases(availableLetters, currentPhrase, childNode.Value);

        //        currentPhrase.Pop();
        //        availableLetters[childNode.Key]++;
        //    }
        //}

        private IEnumerable<string> FindPhrases(Dictionary<char, int> availableLetters, StringBuilder currentPhrase, TrieNode currentNode)
        {
            if (currentNode.IsTerminal)
            {
                if (currentPhrase.WhiteSpaceCount() > MaxWords - 1)
                {
                    yield break;
                }

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
                if (availableLetters[childNode.Key] == 0)
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
