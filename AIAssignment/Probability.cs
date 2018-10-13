// Project: AIAssignment
// Filename; Probability.cs
// Created; 10/10/2018
// Edited: 11/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    public class Probability
    {
        /// <summary>
        /// The word.
        /// </summary>
        private string m_Word;

        /// <summary>
        /// The count of occurrences
        /// </summary>
        private int m_Count;

        /// <summary>
        /// The probability of the word occurring
        /// </summary>
        private double m_Probability;

        /// <summary>
        /// Initializes a new instance of the <see cref="Probability"/> class.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="count">
        /// The count of occurrences
        /// </param>
        /// <param name="probability">
        /// The probability of the word occurring
        /// </param>
        public Probability(string word, int count, double probability)
        {
            this.m_Word = word;
            this.m_Count = count;
            this.m_Probability = probability;
        }

        public Probability()
        {

        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        public string Word
        {
            get => m_Word;
            set => this.m_Word = value;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get => m_Count;
            set => this.m_Count = value;
        }

        /// <summary>
        /// Gets the probability of occurrence.
        /// </summary>
        public double ProbabilityOfOccurrence
        {
            get => m_Probability;
            set => this.m_Probability = value;
        }

        /// <summary>
        /// The word, count, probability laid out as such
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> of word, count, probability.
        /// </returns>
        public override string ToString()
        {
            return this.m_Word + ", " + this.m_Count + ", " + this.m_Probability;
        }

        public void LogirithmProbability()
        {
            this.m_Probability = Math.Log(this.m_Probability);
        }
    }
}