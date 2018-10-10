﻿// Project: AIAssignment
// Filename; Category.cs
// Created; 10/10/2018
// Edited: 11/10/2018

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
        private List<Speech> m_CategorySpeeches = new List<Speech>();

        /// <summary>
        /// Probability of the speech being this party/category
        /// </summary>
        private double m_Probability;

        /// <summary>
        /// Contains all the words for all the speeches in the party/category
        /// </summary>
        private readonly Dictionary<string, int> m_CategoryWordsDictionary = new Dictionary<string, int>();

        /// <summary>
        /// Contains all the words and the counts with the P(word|category)
        /// </summary>
        private readonly List<Probability> m_WordProbabilities = new List<Probability>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class. 
        /// </summary>
        /// <param name="name">
        /// Name of the Category/Party
        /// </param>
        public Category(string name)
        {
            this.m_CategoryName = name;
        }

        /// <summary>
        /// Gets or sets the category speeches.
        /// </summary>
        public List<Speech> CategorySpeeches
        {
            get => this.m_CategorySpeeches;
            set => this.m_CategorySpeeches = value;
        }

        /// <summary>
        /// Gets or sets the probability.
        /// </summary>
        public double Probability
        {
            get => m_Probability;
            set => m_Probability = value;
        }

        /// <summary>
        /// Gets the category words dictionary.
        /// </summary>
        public Dictionary<string, int> CategoryWordsDictionary
        {
            get => this.m_CategoryWordsDictionary;
        }

        /// <summary>
        /// Gets the word probabilities.
        /// </summary>
        public List<Probability> WordProbabilities
        {
            get => m_WordProbabilities;
        }

        /// <summary>
        /// ToString to return the name of the category
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> name.
        /// </returns>
        public override string ToString()
        {
            return this.m_CategoryName;
        }

        /// <summary>
        /// Takes the speeches and adds there words to the word dictionary in the category
        /// </summary>
        public void CondenseSpeeches()
        {
            foreach (Speech speech in this.m_CategorySpeeches)
            {
                //Gets the word dictionaries from the speeches and adds them together
                foreach (KeyValuePair<string, int> pair in speech.WordsDictionary)
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

        /// <summary>
        /// The calculate word prob.
        /// </summary>
        /// <param name="TotalWords">
        /// The total unique words.
        /// </param>
        public void CalculateWordProb(int TotalWords)
        {
            foreach (KeyValuePair<string, int> pair in CategoryWordsDictionary)
            {
                this.m_WordProbabilities.Add(
                    new Probability(
                        pair.Key,
                        pair.Value,
                        BayesianCalculator.WordProbability(
                            pair.Value,
                            this.m_CategoryWordsDictionary.Sum(x => x.Value),
                            TotalWords)));
            }
        }
    }
}