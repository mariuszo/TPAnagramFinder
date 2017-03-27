using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Numerics;

namespace TPAnagramFinder.Tests
{
    [TestClass]
    public class AnagramGeneratorTests
    {
        [TestMethod]
        public void Test_GetSentenceByKeys()
        {
            var dict = new[] { "a", "tea", "ate", "eat" };
            var generator = new AnagramGenerator(dict, 1);
            var phrase = "aaet";
            generator.Initialize(phrase);

            var actual = generator.GetSentenceByKeys(new[] { "a", "aet" }).ToList();

            Assert.AreEqual(3, actual.Count);
            CollectionAssert.AreEquivalent(new[] { "a eat", "a tea", "a ate" }, actual);
        }

        [TestMethod]
        public void Test_GenerateVectorCombinations()
        {
            var dict = new[] { "a", "tea", "ate", "eat" };
            var generator = new AnagramGenerator(dict, 1);
            var phrase = "aaet";
            generator.Initialize(phrase);
            var expected = new[]
            {
                new [] { CreateVector(1), CreateVector(1, 1, 1) },
                new [] { CreateVector(1, 1, 1), CreateVector(1) }
            }
            .SelectMany(c => c)
            .ToList();
            
            var actual = generator.GenerateVectorCombinations().ToList();

            Assert.AreEqual(2, actual.Count);
            CollectionAssert.AreEquivalent(expected, actual.SelectMany(c => c).ToList());
        }

        [TestMethod]
        public void Test_ConvertVectorCombinationsToKeyCombinations()
        {
            var dict = new[] { "a", "tea", "ate", "eat" };
            var generator = new AnagramGenerator(dict, 1);
            var phrase = "aaet";
            generator.Initialize(phrase);
            var combinations = new[]
            {
                new [] { CreateVector(1), CreateVector(1, 1, 1) },
                new [] { CreateVector(1, 1, 1), CreateVector(1) }
            };

            var expected = new[]
            {
                new [] { "a", "aet" },
                new [] { "aet", "a" }
            }
            .ToList();

            var actual = generator.ConvertVectorCombinationsToKeyCombinations(combinations).ToList();

            Assert.AreEqual(2, actual.Count);

            CollectionAssert.AreEquivalent(expected.SelectMany(w => w).ToList(), actual.SelectMany(w => w).ToList());
        }

        private Vector<byte> CreateVector(params byte[] values)
        {
            var b = new byte[Vector<byte>.Count];
            for(var i = 0; i < values.Length; i++)
            {
                b[i] = values[i];
            }
            return new Vector<byte>(b);
        }
    }
}
