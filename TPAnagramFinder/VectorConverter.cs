using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TPAnagramFinder
{
    public class VectorConverter
    {
        private readonly char[] _availableCharacters;
        private readonly Dictionary<int, char> _charMap;
        private readonly Dictionary<char, int> _indexMap;

        public VectorConverter(string seed)
        {
            var charCounts = seed
                .OrderBy(c => c)
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            _availableCharacters = charCounts.Select(c => c.Key).OrderBy(c => c).ToArray();

            _charMap = Enumerable.Range(0, _availableCharacters.Length).ToDictionary(index => index, index => _availableCharacters[index]);
            _indexMap = Enumerable.Range(0, _availableCharacters.Length).ToDictionary(index => _availableCharacters[index], index => index);
        }

        public Vector<byte> CovertString(string input)
        {
            if (input.Any(l => !_availableCharacters.Contains(l)))
                throw new InvalidOperationException($"Input {input} contains invalid characters");

            var result = new byte[Vector<byte>.Count];
            foreach(var letter in input)
            {
                result[_indexMap[letter]]++;
            }

            return new Vector<byte>(result);
        }

        public string GetStringValue(Vector<byte> input)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Vector<byte>.Count; i++)
            {
                for(var j = 0; j < input[i]; j++)
                {
                    sb.Append(_charMap[i]);
                }
            }
            return sb.ToString();
        }
    }
}
