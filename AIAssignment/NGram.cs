// Project: AIAssignment
// Filename; NGram.cs
// Created; 19/10/2018
// Edited: 17/11/2018

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
        /// <summary>
        /// Contains all the possible n-grams
        /// </summary>
        private static readonly string[] m_NGrams = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Ngrams.txt");

        /// <summary>
        /// Creates the a dictionary of the n-grams contained within the speech with the count of the occurrences
        /// </summary>
        /// <param name="script">The string array of all the words in the script</param>
        /// <returns>Dictionary of all the n-grams with the count of occurrences</returns>
        public static Dictionary<string, int> CreateNGramFromScript(string[] script)
        {
            //Remove the / from the document
            RemoveSpliter();

            Dictionary<string, int> nGramDictionary = new Dictionary<string, int>();

            //Add the words found in the n-gram array to the dictionary
            for (int i = 1; i < script.Length; i++)
            {
                if (m_NGrams.Any(x => x == (script[i] + " " + script[i - 1])))
                {
                    string NGramString = script[i - 1] + " " + script[i];
                    if (!nGramDictionary.ContainsKey(NGramString))
                    {
                        nGramDictionary.Add(NGramString, 1);
                    }
                    else
                    {
                        nGramDictionary[NGramString]++;
                    }
                }
            }

            return nGramDictionary;
        }

        /// <summary>
        /// Removes the / from the document that is used to split the words
        /// </summary>
        private static void RemoveSpliter()
        {
            for (int i = 0; i < m_NGrams.Length; i++)
            {
                m_NGrams[i] = m_NGrams[i].Replace('/', ' ');
            }
        }
    }
}