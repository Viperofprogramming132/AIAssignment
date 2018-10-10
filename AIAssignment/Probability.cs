using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    public class Probability
    {
        private string m_Word;

        private int m_Count;

        private double m_Probability;

        public Probability(string word, int count, double probability)
        {
            this.m_Word = word;
            this.m_Count = count;
            this.m_Probability = probability;
        }

        public string Word
        {
            get => m_Word;
        }

        public int Count
        {
            get => m_Count;
        }

        public double ProbabilityOfOccurrence
        {
            get => m_Probability;
        }

        public override string ToString()
        {
            return this.m_Word + ", " + this.m_Count + ", " + this.m_Probability;
        }
    }
}
