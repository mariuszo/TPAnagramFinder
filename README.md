# Anagram Finder (TrustPilot challenge)
The objective - given a list of words, initial phrase, and md5 hashes of target phrases, find those phrases, which are anagrams of initial phrase.
## Solution
Application manages to find target phrases for all three levels of difficulty, however runtime is a bit too long although reasonable. But there are possible improvements.
## Implementation considerations
1. First and foremost it is important to reduce problem size, by filtering initial dictionary.
  * Dictionary contains quite a few duplicates.
  * Words, containing letters, which are not a subset of letters from initial phrase, can be removed.
  * There are a few one letter non-words.
2. Initial solution was to build a Trie from dictionary and use depth-first search to generate word combinations, however it appeared to be unacceptably slow.
3. Most difficult problem was to generate all possible combinations and permutations of N words, which use up all letters from initial phrase.
  * An index was built from wordlist, where each key was letters of a word, arranged in ascending order, and value - list of words made up from those letters. This allowed to further reduce amount of combinations, as only dictionary keys had to be checked against available letter inventory, and words could be generated in later step.
  * Generating dictionary key combintations appeared to be still to lengthy, therefore it was further optimized by converting dictionary keys and initial phrase to byte vectors.
  * Vectors allow quick subtractions and comparisons, because modern CPUs support SIMD operations.
4. After generating key combinations, it is afterwards trivial to convert key combinations into phrases by fetching words for each key from initial dictionary index.
## Future work
* The most important and performance intensive step - generating key vector combination, is the slowest, as it is performed in sequential manner, therefore further performance improvement could be achieved by parallelizing this step.
