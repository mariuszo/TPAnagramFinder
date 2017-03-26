using System.Linq;
using System.Text;

namespace TPAnagramFinder
{
    public static class Extensions
    {
        public static bool IsSubSet(this string word, string set)
        {
            return !word.Except(set).Any();
        }

        public static StringBuilder Pop(this StringBuilder builder)
        {
            return builder.Remove(builder.Length - 1, 1);
        }
    }
}
