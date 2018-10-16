// Project: AIAssignment
// Filename; Probability.cs
// Created; 10/10/2018
// Edited: 16/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment.Network
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
        /// Term Frequency Inverse Document Frequency
        /// </summary>
        private double m_TFIDF;

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

        /// <summary>
        /// Blank constructor for serialization
        /// </summary>
        public Probability()
        {
        }

        /// <summary>
        /// Gets the word.
        /// </summary>
        public string Word
        {
            get => this.m_Word;
            set => this.m_Word = value;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get => this.m_Count;
            set => this.m_Count = value;
        }

        /// <summary>
        /// Gets the probability of occurrence.
        /// </summary>
        public double ProbabilityOfOccurrence
        {
            get => this.m_Probability;
            set => this.m_Probability = value;
        }

        /// <summary>
        /// Probability based off the frequency the word is used therefore more common words have less effect
        /// </summary>
        public double TermFrequencyProbability
        {
            get => this.m_TFIDF;
            set => this.m_TFIDF = value;
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

        /// <summary>
        /// Generates the term frequency inverse document frequency
        /// </summary>
        /// <param name="totalWords">Total words in the document</param>
        /// <param name="totalScripts">Total Scripts in corpus</param>
        /// <param name="scriptsContainingWord">The count of the scripts that are containing the word</param>
        public void CalculateTFIDF(int totalWords, int totalScripts, int scriptsContainingWord)
        {
            double TFword = (double)this.m_Count / totalWords;
            double IF = 1 + Math.Log((double)totalScripts / scriptsContainingWord);

            if (IF != 0)
            {
                this.m_TFIDF = TFword / IF;
            }
            else
            {
                this.m_TFIDF = this.m_Probability;
            }
        }
    }
}