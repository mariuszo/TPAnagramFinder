using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TPAnagramFinder
{
    public class PermutationsGenerator
    {
        private readonly List<char> _values;
        private readonly int[] _lexicographicOrder;

        private char _tempVal;
        private int _tempIndex;

        public string Value
        {
            get
            {
                return new string(_values.ToArray());
            }
        }

        public PermutationsGenerator(string value)
        {
            _values = new List<char>(value);
            _lexicographicOrder = new int[_values.Count];

            _values.Sort();
            var j = 1;
            if(_lexicographicOrder.Length > 0)
            {
                _lexicographicOrder[0] = j;
            }
            for(var i = 1; i < _lexicographicOrder.Length; ++i)
            {
                if(_values[i - 1] != _values[i])
                {
                    ++j;
                }
                _lexicographicOrder[i] = j;
            }
        }

        public bool NextPermutation()
        {
            var i = _lexicographicOrder.Length - 1;
            while(_lexicographicOrder[i - 1] >= _lexicographicOrder[i])
            {
                --i;
                if (i == 0) return false;
            }

            var j = _lexicographicOrder.Length;
            while(_lexicographicOrder[j - 1] <= _lexicographicOrder[i - 1])
            {
                --j;
            }
            Swap(i - 1, j - 1);
            ++i;
            j = _lexicographicOrder.Length;
            while(i < j)
            {
                Swap(i - 1, j - 1);
                ++i;
                --j;
            }
            return true;
        }

        private void Swap(int i, int j)
        {
            _tempVal = _values[i];
            _values[i] = _values[j];
            _values[j] = _tempVal;
            _tempIndex = _lexicographicOrder[i];
            _lexicographicOrder[i] = _lexicographicOrder[j];
            _lexicographicOrder[j] = _tempIndex;
        }
    }
}
