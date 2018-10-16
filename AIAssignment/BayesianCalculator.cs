// Project: AIAssignment
// Filename; BayesianCalculator.cs
// Created; 10/10/2018
// Edited: 16/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment.Network
{
    /// <summary>
    /// Calculator that does all the mathematics for the bayesain probability
    /// </summary>
    public static class BayesianCalculator
    {
        /// <summary>
        /// Calculates the probability of a category occurring
        /// </summary>
        /// <param name="numOfCategory">Total number of categories/parties</param>
        /// <param name="totalDocs">Total number of speeches</param>
        /// <returns>Double of P(Category)</returns>
        public static double ProbabilityOfCategory(int numOfCategory, int totalDocs)
        {
            return (double)numOfCategory / totalDocs;
        }

        /// <summary>
        /// Calculates the probability of word where category has occurred
        /// </summary>
        /// <param name="numOfWord">Number of occurrences of the word in the Category</param>
        /// <param name="totalCategoryWords">Total number of words (including repeats) in the Category</param>
        /// <param name="totalWords">Total number of unique words</param>
        /// <returns>Double of P(word|Category)</returns>
        public static double WordProbability(int numOfWord, int totalCategoryWords, int totalWords)
        {
            return (double)(numOfWord + 1) / (totalCategoryWords + totalWords);
        }

        /// <summary>
        /// Calculates the probability of the document being the category
        /// </summary>
        /// <param name="wordProbabilities">List of the common words and there probabilities of occurring between the document and the category</param>
        /// <param name="categoryProbability">Probability of the category occurring</param>
        /// <returns>probability of the new document occurring</returns>
        public static double DocumentProbability(List<Probability> wordProbabilities, double categoryProbability)
        {
            double docProbability = Math.Log(wordProbabilities[0].TermFrequencyProbability);
            wordProbabilities.RemoveAt(0);

            foreach (Probability probability in wordProbabilities)
            {
                docProbability += Math.Log(probability.TermFrequencyProbability);
            }

            docProbability += Math.Log(categoryProbability);

            return docProbability;
        }
    }
}