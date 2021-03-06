﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TPAnagramFinder
{
    /// <summary>
    /// Class which allows to convert strings into byte vectors and vice-versa.
    /// Each char from seed string is mapped to a index value.
    /// For each converted string, number of characters in that string is
    /// set as a element of a vector, with index corresponding index map from seed string.
    /// E.g.
    ///   seed string - "abc"
    ///   input string - "aabc"
    ///   vector - {2, 1, 1}
    ///   
    ///   input string - "bcc"
    ///   vector - {0, 1, 2}
    /// </summary>
    public class VectorConverter
    {
        private readonly char[] _availableCharacters;
        private readonly Dictionary<int, char> _charMap;
        private readonly Dictionary<char, int> _indexMap;

        public VectorConverter(string seed)
        {
            _availableCharacters = seed.Distinct().OrderBy(c => c).ToArray();

            _charMap = Enumerable.Range(0, _availableCharacters.Length).ToDictionary(index => index, index => _availableCharacters[index]);
            _indexMap = Enumerable.Range(0, _availableCharacters.Length).ToDictionary(index => _availableCharacters[index], index => index);
        }

        public Vector<byte> CovertString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

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
