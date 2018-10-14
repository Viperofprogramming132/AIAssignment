using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    /// <summary>
    /// Reduces the words down to the simplest form (makes words not proper words though)
    /// Algorithm from http://snowballstem.org/algorithms/english/stemmer.html
    /// </summary>
    public class Stemmer
    {
        /// <summary>
        /// Gets the alphabet
        /// https://stackoverflow.com/questions/314466/generating-an-array-of-letters-in-the-alphabet
        /// </summary>
        private readonly char[] m_Alphabet =
            Enumerable.Range('a', 'z' - 'a' + 1)
            .Select(c => (char)c)
            .Concat(new[] { '\'' }).ToArray();


        /// <summary>
        /// Char array for the vowels of the alphabet
        /// </summary>
        private readonly char[] m_Vowels = "aeiou".ToCharArray();

        /// <summary>
        /// Contains all the endings that will be replaced to Li
        /// </summary>
        private readonly char[] m_LiEndings = "cdeghkmnrt".ToCharArray();

        /// <summary>
        /// if there is a double remove the last letter
        /// </summary>
        private readonly string[] m_Doubles = { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };

        /// <summary>
        /// Check if the char is a vowel
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if vowel, false if not</returns>
        private bool IsVowel(char c)
        {
            return this.m_Vowels.Contains(c);
        }

        /// <summary>
        /// Check if the char is not a vowel
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>False if vowel, false if it is a vowel</returns>
        private bool IsConsonant(char c)
        {
            return !this.m_Vowels.Contains(c);
        }

        /// <summary>
        /// Check if r1 is less than or equal to the word length - the suffix length
        /// </summary>
        /// <param name="word">word to check</param>
        /// <param name="r1"></param>
        /// <param name="suffix">Suffix to check</param>
        /// <returns></returns>
        private bool SuffixInR1(string word, int r1, string suffix)
        {
            return r1 <= word.Length - suffix.Length;
        }

        /// <summary>
        /// Check if r2 is less than or equal to the word length - the suffix length
        /// </summary>
        /// <param name="word">word to check</param>
        /// <param name="r2"></param>
        /// <param name="suffix">Suffix to check</param>
        /// <returns></returns>
        private bool SuffixInR2(string word, int r2, string suffix)
        {
            return r2 <= word.Length - suffix.Length;
        }

        /// <summary>
        /// Reduces the words down to the simplest form
        /// </summary>
        /// <param name="word">The word to simplify</param>
        /// <returns>The simplest word</returns>
        public string StemWord(string word)
        {
            if (word.Length <= 2)
            {
                return word;
            }

            word = word.ToLower();

            int r1 = this.GetRegion(word, 0);
            int r2 = this.GetRegion(word, r1);

            word = this.RemoveSSuffix(word);
            word = this.RemoveOtherSSuffix(word);
            word = this.RemoveLySuffix(word, r1);
            word = this.ReplaceYwithIWhereFollowingConsonant(word);
            word = this.ReplaceSuffixes(word, r1);
            word = this.ReplaceAdditionalSuffixes(word, r1, r2);
            word = this.RemoveSuffixesInR2(word, r2);
            word = this.RemoveEorLSuffixes(word, r1, r2);

            return word;
        }

        private int GetRegion(string word, int begin)
        {
            bool vowel = false;

            for (int i = begin; i < word.Length; i++)
            {
                if (this.IsVowel(word[i]))
                {
                    vowel = true;
                    continue;
                }

                if (vowel && this.IsConsonant(word[i]))
                {
                    return i + 1;
                }
            }

            return word.Length;
        }

        private bool IsShortWord(string word)
        {
            return this.EndsInShortSyllable(word) && this.GetRegion(word, 0) == word.Length;
        }

        private bool EndsInShortSyllable(string word)
        {
            if (word.Length < 2)
            {
                return false;
            }

            // a vowel at the beginning of the word followed by a non-vowel
            if (word.Length == 2)
            {
                return this.IsVowel(word[0]) && this.IsConsonant(word[1]);
            }

            return this.IsVowel(word[word.Length - 2])
                   && this.IsConsonant(word[word.Length - 1])
                   && this.IsConsonant(word[word.Length - 3]);
        }
        private bool TryReplace(string word, string oldSuffix, string newSuffix, out string final)
        {
            if (word.Contains(oldSuffix))
            {
                final = this.ReplaceSuffix(word, oldSuffix, newSuffix);
                return true;
            }
            final = word;
            return false;
        }

        private string ReplaceSuffix(string word, string oldSuffix, string newSuffix = null)
        {
            if (oldSuffix != null)
            {
                word = word.Substring(0, word.Length - oldSuffix.Length);
            }

            if (newSuffix != null)
            {
                word += newSuffix;
            }
            return word;
        }

        private string RemoveSSuffix(string word)
        {
            string[] plurals = { "'s", "s", "s'" };
            foreach (string suffix in plurals)
            {
                if (word.EndsWith(suffix))
                {
                    return this.ReplaceSuffix(word, suffix);
                }
            }

            return word;
        }

        private string RemoveOtherSSuffix(string word)
        {
            if (word.EndsWith("sses"))
            {
                return ReplaceSuffix(word, "sses", "ss");
            }

            if (word.EndsWith("ied") || word.EndsWith("ies"))
            {
                string wordWithoutEnd = word.Substring(0, word.Length - 3);
                if (word.Length > 4)
                {
                    return wordWithoutEnd + "i";
                }

                return wordWithoutEnd + "ie";
            }
            if (word.EndsWith("us") || word.EndsWith("ss"))
            {
                return word;
            }
            if (word.EndsWith("s"))
            {
                if (word.Length < 3)
                {
                    return word;
                }

                // Skip both the last letter ('s') and the letter before that
                for (int i = 0; i < word.Length - 2; i++)
                {
                    if (this.IsVowel(word[i]))
                    {
                        return word.Substring(0, word.Length - 1);
                    }
                }
            }

            return word;
        }

        private string RemoveLySuffix(string word, int r1)
        {
            string[] endingString1 = { "eedly", "eed" };
            foreach (string suffix in endingString1.Where(word.EndsWith))
            {
                if (this.SuffixInR1(word, r1, suffix))
                {
                    return this.ReplaceSuffix(word, suffix, "ee");
                }
                return word;
            }

            string[] endingString2 = { "ed", "edly", "ing", "ingly" };

            foreach (string suffix in endingString2.Where(word.EndsWith))
            {
                string trunc = this.ReplaceSuffix(word, suffix);
                if (trunc.Any(this.IsVowel))
                {
                    if (new string[] { "at", "bl", "iz" }.Any(trunc.EndsWith))
                    {
                        return trunc + "e";
                    }
                    if (this.m_Doubles.Any(trunc.EndsWith))
                    {
                        return trunc.Substring(0, trunc.Length - 1);
                    }
                    if (this.IsShortWord(trunc))
                    {
                        return trunc + "e";
                    }
                    return trunc;
                }
                return word;
            }

            return word;
        }

        private string ReplaceYwithIWhereFollowingConsonant(string word)
        {
            if (word.EndsWith("y")
                && word.Length > 2
                && IsConsonant(word[word.Length - 2]))
            {
                return word.Substring(0, word.Length - 1) + "i";
            }
            return word;
        }

        private string ReplaceSuffixes(string word, int r1)
        {
            Dictionary<string, string> suffixes = new Dictionary<string, string>()
                {
                    { "ization", "ize" },
                    { "ational", "ate" },
                    { "ousness", "ous" },
                    { "iveness", "ive" },
                    { "fulness", "ful" },
                    { "tional", "tion" },
                    { "lessli", "less" },
                    { "biliti", "ble" },
                    { "entli", "ent" },
                    { "ation", "ate" },
                    { "alism", "al" },
                    { "aliti", "al" },
                    { "fulli", "ful" },
                    { "ousli", "ous" },
                    { "iviti", "ive" },
                    { "enci", "ence" },
                    { "anci", "ance" },
                    { "abli", "able" },
                    { "izer", "ize" },
                    { "ator", "ate" },
                    { "alli", "al" },
                    { "bli", "ble" }
                };

            foreach (KeyValuePair<string,string> suffix in suffixes)
            {
                if (word.EndsWith(suffix.Key))
                {
                    string final;
                    if (SuffixInR1(word, r1, suffix.Key)
                        && TryReplace(word, suffix.Key, suffix.Value, out final))
                    {
                        return final;
                    }
                    return word;
                }
            }


            if (word.EndsWith("ogi")
                && SuffixInR1(word, r1, "ogi")
                && word[word.Length - 4] == 'l')
            {
                return this.ReplaceSuffix(word, "ogi", "og");
            }

            if (word.EndsWith("li") & SuffixInR1(word, r1, "li"))
            {
                if (this.m_LiEndings.Contains(word[word.Length - 3]))
                {
                    return this.ReplaceSuffix(word, "li");
                }
            }

            return word;

        }

        private string ReplaceAdditionalSuffixes(string word, int r1, int r2)
        {
            Dictionary<string, string> suffixes = new Dictionary<string, string>
                {
                    {"ational", "ate"},
                    {"tional", "tion"},
                    {"alize", "al"},
                    {"icate", "ic"},
                    {"iciti", "ic"},
                    {"ical", "ic"},
                    {"ful", null},
                    {"ness", null}
                };

            foreach (KeyValuePair<string,string> suffix in suffixes.Where(s => word.EndsWith(s.Key)))
            {
                string final;
                if (this.SuffixInR1(word, r1, suffix.Key)
                    && TryReplace(word, suffix.Key, suffix.Value, out final))
                {
                    return final;
                }
            }

            if (word.EndsWith("ative"))
            {
                if (this.SuffixInR1(word, r1, "ative") && this.SuffixInR2(word, r2, "ative"))
                {
                    return this.ReplaceSuffix(word, "ative", null);
                }
            }

            return word;
        }

        private string RemoveSuffixesInR2(string word, int r2)
        {
            string[] suffixes =
                {
                    "al", "ance", "ence", "er", "ic", "able", "ible", "ant", "ement", "ment", "ent", "ism", "ate",
                    "iti", "ous", "ive", "ize"
                };
            foreach (string suffix in suffixes)
            {
                if (word.EndsWith(suffix))
                {
                    if (this.SuffixInR2(word, r2, suffix))
                    {
                        return this.ReplaceSuffix(word, suffix);
                    }
                    return word;
                }
            }

            if (word.EndsWith("ion") &&
                this.SuffixInR2(word, r2, "ion") &&
                new[] { 's', 't' }.Contains(word[word.Length - 4]))
            {
                return this.ReplaceSuffix(word, "ion");
            }
            return word;
        }

        private string RemoveEorLSuffixes(string word, int r1, int r2)
        {
            if (word.EndsWith("e") &&
                (this.SuffixInR2(word, r2, "e") ||
                 (this.SuffixInR1(word, r1, "e") &&
                  !EndsInShortSyllable(this.ReplaceSuffix(word, "e")))))
            {
                return this.ReplaceSuffix(word, "e");
            }

            if (word.EndsWith("l") &&
                this.SuffixInR2(word, r2, "l") &&
                word.Length > 1 &&
                word[word.Length - 2] == 'l')
            {
                return this.ReplaceSuffix(word, "l");
            }

            return word;
        }
    }
}
