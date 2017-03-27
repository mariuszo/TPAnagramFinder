using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPAnagramFinder
{
    public class Program
    {
        private const string PhraseHashEasy = "e4820b45d2277f3844eac66c903e84be";
        private const string PhraseHashMedium = "23170acc097c24edb98fc5488ab033fe";
        private const string PhraseHashHard = "665e5bcb0c20062fe8abaaf4628bb154";

        private const string Phrase = "poultry outwits ants";

        //also input towy ruts
        private const string TestHash = "8c2ea82e9a1d56db0d597e5798307a4a";

        private static ThreadLocal<MD5> _md5;

        private static void Main(string[] args)
        {
            var wordlist = File.ReadAllLines(args[0]);
            _md5 = new ThreadLocal<MD5>(() => MD5.Create());

            var testHashMatches = GetHashMatcher(GetHashStringBytes(TestHash));
            var easyHashMatches = GetHashMatcher(GetHashStringBytes(PhraseHashEasy));
            var mediumHashMatches = GetHashMatcher(GetHashStringBytes(PhraseHashMedium));
            var hardHashMatches = GetHashMatcher(GetHashStringBytes(PhraseHashHard));

            var sw = Stopwatch.StartNew();

            //var trie = new TrieAnagramSolver(wordlist, Phrase);
            //foreach (var phrase in trie.FindAnagrams())
            //{
            //    if (mediumHashMatches(phrase))
            //    {
            //        LogResult($"MED:{phrase}, elapsed: {sw.Elapsed}");
            //        break;
            //    }
            //}

            var results = new ConcurrentBag<string>();
            long found = 0;

            var solver = new AnagramGenerator3000(wordlist, 1, 4);
            solver.BuildDictionary(Phrase);
            var combos = solver.GenerateVectorCombinations().ToList();
            var keys = solver.ConvertVectorCombinationsToKeyCombinations(combos).ToList();
            var sentences = solver.GetSentences(keys).ToList();

            Parallel.ForEach(sentences, (sentence, loopState) =>
            {
                if(easyHashMatches(sentence))
                {
                    results.Add($"EASY:{sentence}");
                    Interlocked.Increment(ref found);
                }
                if(mediumHashMatches(sentence))
                {
                    results.Add($"MED:{sentence}");
                    Interlocked.Increment(ref found);
                }
                if (hardHashMatches(sentence))
                {
                    results.Add($"HARD:{sentence}");
                    Interlocked.Increment(ref found);
                }
                if (Interlocked.Read(ref found) == 3) loopState.Stop();
            });

            sw.Stop();

            foreach(var result in results)
            {
                LogResult(result);
            }

            Console.WriteLine($"Done, elapsed: {sw.Elapsed}");
            Console.ReadKey();
        }
        
        private static void LogResult(string result)
        {
            Console.WriteLine(result);

            using (var writer = File.AppendText("results.txt"))
            {
                writer.WriteLine(result);
            }
        }

        private static Func<string, bool> GetHashMatcher(byte[] hashBytes)
        {
            return (input) => _md5.Value.ComputeHash(Encoding.ASCII.GetBytes(input)).SequenceEqual(hashBytes);
        }

        private static byte[] GetHashStringBytes(string hash)
        {
            return Enumerable.Range(0, hash.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hash.Substring(x, 2), 16))
                     .ToArray();
        }        
    }
}