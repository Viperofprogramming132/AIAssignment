using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment.Network
{
    public static class NGram
    {
        private static string[] m_NGrams = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Ngrams.txt");
        public static Dictionary<string, int> CreateNGramFromScript(string script)
        {
            RemoveSpliter();
            string[] words = script.Split(' ', '\n');
            Dictionary<string, int> NGramDictionary = new Dictionary<string, int>();
            for (int i = 1; i < words.Length; i++)
            {
                if (m_NGrams.Any(x => x == (words[i] + " " + words[i-1])))
                {
                    string NGramString = words[i - 1] + " " + words[i];
                    if (!NGramDictionary.ContainsKey(NGramString))
                    {
                        NGramDictionary.Add(NGramString, 1);
                    }
                    else
                    {
                        NGramDictionary[NGramString]++;
                    }
                }

            }

            return NGramDictionary;
        }

        private static void RemoveSpliter()
        {
            for(int i = 0; i < m_NGrams.Length; i++)
            {
                m_NGrams[i] = m_NGrams[i].Replace('/',' ');
            }
        }
    }
}
