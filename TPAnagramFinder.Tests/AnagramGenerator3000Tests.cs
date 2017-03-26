using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPAnagramFinder.Tests
{
    [TestClass]
    public class AnagramGenerator3000Tests
    {
        [TestMethod]
        public void Test_GetKeyCombinations1()
        {
            var dict = new[] { "a", "tea", "ate", "eat" };
            var generator = new AnagramGenerator3000(dict, 1);
            var phrase = "aaet";
            generator.BuildDictionary(phrase);

            var expected = new[]
            {
                new [] { "a", "aet" },
                new [] { "aet", "a" }
            }.OrderBy(_ => _.First())
            .ToList();

            var actual = generator.GetKeyCombinations(generator.BuildLetterInventory(phrase)).OrderBy(_ => _.First()).ToList();

            Assert.AreEqual(2, actual.Count);
            for(var i = 0; i < actual.Count; i++)
            {
                CollectionAssert.AreEquivalent(expected[i], actual[i].ToList());
            }
        }

        //[TestMethod]
        //public void Test_GetKeyCombinations2()
        //{
        //    var dict = new[] { "a", "tea", "ate", "eat" };
        //    var generator = new AnagramGenerator3000(dict, 1);
        //    var phrase = "aaet";
        //    generator.BuildDictionary(phrase);

        //    var actual = generator.GetKeyCombinations(phrase, new List<string>()).ToList();

        //    Assert.AreEqual(2, actual.Count);
        //}

        [TestMethod]
        public void Test_GetSentenceByKeys()
        {
            var dict = new[] { "a", "tea", "ate", "eat" };
            var generator = new AnagramGenerator3000(dict, 1);
            var phrase = "aaet";
            generator.BuildDictionary(phrase);

            var actual = generator.GetSentenceByKeys(new[] { "a", "aet" }).ToList();

            Assert.AreEqual(3, actual.Count);
            CollectionAssert.AreEquivalent(new[] { "a eat", "a tea", "a ate" }, actual);
        }
    }
}
