using System.Linq;
using System.Numerics;

namespace TPAnagramFinder
{
    public static class Extensions
    {
        public static bool IsSubSet(this string word, string set)
        {
            return !word.Except(set).Any();
        }

        public static byte GetVectorComponentSum(this Vector<byte> vector)
        {
            return Vector.Dot(vector, Vector<byte>.One);
        }
    }
}
