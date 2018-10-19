using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment.Network
{
    public static class NGram
    {
        public static Dictionary<string,int> CreateNGramFromScript(string script)
        {
            string[] words = script.Split(' ','\n');
            Dictionary<string, int> NGramDictionary = new Dictionary<string, int>();
            for (int i = 1; i < words.Length; i++)
            {
                string NGramString = words[i - 1] + " " + words[i];
                if(!NGramDictionary.ContainsKey(NGramString))
                {
                    NGramDictionary.Add(NGramString, 1);
                }
                else
                {
                    NGramDictionary[NGramString]++;
                }
                
            }

            return NGramDictionary;
        }
    }
}
