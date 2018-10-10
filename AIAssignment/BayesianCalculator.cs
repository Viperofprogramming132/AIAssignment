﻿// Project: AIAssignment
// Filename; BayesianCalculator.cs
// Created; 10/10/2018
// Edited: 11/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
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
    }
}