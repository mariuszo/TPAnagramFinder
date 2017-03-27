using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TPAnagramFinder.Tests
{
    [TestClass]
    public class VectorConverterTests
    {
        [TestMethod]
        public void Test_CovertString()
        {
            var seed = "ate";
            var input = "aate";
            var converter = new VectorConverter(seed);

            var actual = converter.CovertString(input);

            var expectedBytes = new byte[Vector<byte>.Count];
            expectedBytes[0] = 2;
            expectedBytes[1] = 1;
            expectedBytes[2] = 1;
            var expected = new Vector<byte>(expectedBytes);

            Assert.IsTrue(expected == actual);
        }

        [TestMethod]
        public void Test_GetStringValue()
        {
            var seed = "ate";
            var inputBytes = new byte[Vector<byte>.Count];
            inputBytes[0] = 2;
            inputBytes[1] = 1;
            inputBytes[2] = 1;
            var input = new Vector<byte>(inputBytes);
            var converter = new VectorConverter(seed);

            var actual = converter.GetStringValue(input);

            var expected = "aaet";

            Assert.AreEqual(expected, actual);
        }
    }
}
