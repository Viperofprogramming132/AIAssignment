using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment
{
    

    public class Category
    {
        /// <summary>
        /// Party/Category name
        /// </summary>
        private readonly string m_CategoryName;

        /// <summary>
        /// Contains all speeches for this party/category
        /// </summary>
        private List<Speech> m_CatagorySpeeches = new List<Speech>();

        /// <summary>
        /// Probability of the speech being this party/category
        /// </summary>
        private double m_Probability;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, int> m_CategoryWordsDictionary = new Dictionary<string, int>();

        private List<Probability> m_WordProbabilities = new List<Probability>();

        public Category(string name)
        {
            this.m_CategoryName = name;
        }

        public List<Speech> CatagorySpeeches
        {
            get => this.m_CatagorySpeeches;
            set => this.m_CatagorySpeeches = value;
        }

        public double Probability
        {
            get => m_Probability;
            set => m_Probability = value;
        }

        public Dictionary<string, int> CategoryWordsDictionary
        {
            get => this.m_CategoryWordsDictionary;
        }

        public List<Probability> WordProbabilities
        {
            get => m_WordProbabilities;
        }

        public override string ToString()
        {
            return this.m_CategoryName;
        }

        public void CondenseSpeeches()
        {
            foreach (Speech speech in this.m_CatagorySpeeches)
            {
                //Gets the word dictionaries from the speeches and adds them together
                foreach (KeyValuePair<string,int> pair in speech.WordsDictionary)
                {
                    if (this.m_CategoryWordsDictionary.ContainsKey(pair.Key))
                    {
                        this.m_CategoryWordsDictionary[pair.Key] += pair.Value;
                    }
                    else
                    {
                        this.m_CategoryWordsDictionary.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        public void CalculateWordProb(int TotalWords)
        {
            foreach (KeyValuePair<string,int> pair in CategoryWordsDictionary)
            {
                this.m_WordProbabilities.Add(new Probability(pair.Key,pair.Value,BayesianCalculator.WordProbability(pair.Value, this.m_CategoryWordsDictionary.Sum(x => x.Value), TotalWords)));
            }

            double totalProbability = 0;

            foreach (Probability p in m_WordProbabilities)
            {
                totalProbability += p.ProbabilityOfOccurrence;
            }
        }
    }
}
