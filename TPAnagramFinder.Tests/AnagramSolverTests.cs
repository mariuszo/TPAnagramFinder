using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TPAnagramFinder.Tests
{
    [TestClass]
    public class AnagramSolverTests
    {
        private AnagramSolver _solver;
        private IEnumerable<string> _wordlist;

        [TestInitialize]
        public void SetUp()
        {
            _wordlist = File.ReadAllLines("C:\\Users\\mariu\\Desktop\\wordlist");
        }

        [TestMethod]
        public void Test_GetWordOccurences_abcd()
        {
            _solver = new AnagramSolver(_wordlist, "abcd");
            var actual = _solver.GetWordOccurences("abcd");

            var expected = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('b', 1),
                new Occurence('c', 1),
                new Occurence('d', 1)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_GetWordOccurences_Robert()
        {
            _solver = new AnagramSolver(_wordlist, "robert");
            var actual = _solver.GetWordOccurences("Robert");

            var expected = new List<Occurence>
            {
                new Occurence('b', 1),
                new Occurence('e', 1),
                new Occurence('o', 1),
                new Occurence('r', 2),
                new Occurence('t', 1)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_GetSentenceOccurences()
        {
            _solver = new AnagramSolver(_wordlist, "abcde");
            var actual = _solver.GetSentenceOccurences("abcd e");

            var expected = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('b', 1),
                new Occurence('c', 1),
                new Occurence('d', 1),
                new Occurence('e', 1)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_DictionaryByOccurences()
        {
            _solver = new AnagramSolver(_wordlist, "ate");
            var key = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('e', 1),
                new Occurence('t', 1)
            };
            var actual = _solver.GetWordsByOccurences(key).ToList();

            var expected = new List<string> { "ate", "eat", "eta", "tea" };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_GetWordAnagrams()
        {
            _solver = new AnagramSolver(_wordlist, "player");
            var expected = new List<string> { "parley", "pearly", "player", "replay", "reaply" };

            var actual = _solver.GetWordAnagrams("player").ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_GetOccurenceCombinations_Empty()
        {
            _solver = new AnagramSolver(_wordlist, "abcd");
            var actual = _solver.GetOccurenceCobinations(new List<Occurence>()).ToList();

            Assert.IsTrue(actual.Any());
            Assert.AreEqual(actual.Count, 1);
            Assert.IsFalse(actual.First().Any());
        }

        [TestMethod]
        public void Test_GetOccurenceCombinations_abba()
        {
            _solver = new AnagramSolver(_wordlist, "abba");
            var abba = new List<Occurence>
            {
                new Occurence('a', 2),
                new Occurence('b', 2)
            };
            var expected = new HashSet<List<Occurence>>(new List<List<Occurence>>
            {
                new List<Occurence>(),
                new List<Occurence> { new Occurence('a', 1) },
                new List<Occurence> { new Occurence('a', 2) },
                new List<Occurence> { new Occurence('b', 1) },
                new List<Occurence> { new Occurence('b', 2) },
                new List<Occurence> { new Occurence('a', 1), new Occurence('b', 1) },
                new List<Occurence> { new Occurence('a', 1), new Occurence('b', 2) },
                new List<Occurence> { new Occurence('a', 2), new Occurence('b', 1) },
                new List<Occurence> { new Occurence('a', 2), new Occurence('b', 2) },
            }, new OccurenceListEqualityComparer());

            var actual = new HashSet<List<Occurence>>(_solver.GetOccurenceCobinations(abba), new OccurenceListEqualityComparer());

            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void Test_SubtractOccurences_lard_r()
        {
            _solver = new AnagramSolver(_wordlist, "adlr");
            var occ1 = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('d', 1),
                new Occurence('l', 1),
                new Occurence('r', 1)
            };
            var occ2 = new List<Occurence>
            {
                new Occurence('r', 1)
            };
            var expected = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('d', 1),
                new Occurence('l', 1)
            };

            var actual = _solver.SubtractOccurences(occ1, occ2).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_SubtractOccurences_lard_empty()
        {
            _solver = new AnagramSolver(_wordlist, "adlr");
            var occ1 = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('d', 1),
                new Occurence('l', 1),
                new Occurence('r', 1)
            };
            var occ2 = new List<Occurence>();
            var expected = new List<Occurence>
            {
                new Occurence('a', 1),
                new Occurence('d', 1),
                new Occurence('l', 1),
                new Occurence('r', 1)
            };

            var actual = _solver.SubtractOccurences(occ1, occ2).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Test_GetSentenceAnagrams()
        {
            _solver = new AnagramSolver(_wordlist, "linux rules");
            var sentence = "Linux rules";
            var expected = new List<string>();

            var actual = _solver.GetSentenceAnagrams(sentence).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
