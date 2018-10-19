﻿// Project: AIAssignment
// Filename; Category.cs
// Created; 10/10/2018
// Edited: 16/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment.Network
{
    public class Category
    {
        /// <summary>
        /// Party/Category name
        /// </summary>
        private string m_CategoryName;

        /// <summary>
        /// Contains all speeches for this party/category
        /// </summary>
        private readonly List<Speech> m_CategorySpeeches = new List<Speech>();

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
        private List<Probability> m_WordProbabilities = new List<Probability>();

        /// <summary>
        /// Contains all the ngram and the counts with the P(word|category)
        /// </summary>
        private List<Probability> m_NGramWordProbabilities = new List<Probability>();

        /// <summary>
        /// Dictionary for containing the Ngrams and their words
        /// </summary>
        Dictionary<string, int> m_NGramDictionary = new Dictionary<string, int>();

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

        public Category()
        {
        }

        /// <summary>
        /// Gets or sets the category speeches.
        /// </summary>
        public List<Speech> GetCategorySpeeches()
        {
            return this.m_CategorySpeeches;
        }

        /// <summary>
        /// Gets or sets the probability.
        /// </summary>
        public double Probability
        {
            get => this.m_Probability;
            set => this.m_Probability = value;
        }

        /// <summary>
        /// Gets the category words dictionary.
        /// </summary>
        public Dictionary<string, int> GetCategoryWordsDictionary()
        {
            return this.m_CategoryWordsDictionary;
        }

        /// <summary>
        /// Gets the category NGram dictionary.
        /// </summary>
        public Dictionary<string, int> GetCategoryNGramDictionary()
        {
            return this.m_NGramDictionary;
        }

        /// <summary>
        /// Gets the word probabilities.
        /// </summary>
        public List<Probability> WordProbabilities
        {
            get => this.m_WordProbabilities;
            set => this.m_WordProbabilities = value;
        }

        /// <summary>
        /// Gets the Ngram probabilities.
        /// </summary>
        public List<Probability> NGramWordProbabilities
        {
            get => this.m_NGramWordProbabilities;
            set => this.m_NGramWordProbabilities = value;
        }

        /// <summary>
        /// Party/Category name
        /// </summary>
        public string CategoryName
        {
            get => this.m_CategoryName;
            set => this.m_CategoryName = value;
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

                foreach (KeyValuePair<string,int> pair in NGram.CreateNGramFromScript(speech.SpeechScript))
                {
                    if (this.m_NGramDictionary.ContainsKey(pair.Key))
                    {
                        this.m_NGramDictionary[pair.Key] += pair.Value;
                    }
                    else
                    {
                        this.m_NGramDictionary.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// The calculate word prob.
        /// </summary>
        /// <param name="totalWords">
        /// The total unique words.
        /// </param>
        /// <param name="totalNGrams">The total unique Ngrams</param>
        public void CalculateWordProb(int totalWords,int totalNGrams)
        {
            foreach (KeyValuePair<string, int> pair in this.m_CategoryWordsDictionary)
            {
                this.m_WordProbabilities.Add(
                    new Probability(
                        pair.Key,
                        pair.Value,
                        BayesianCalculator.WordProbability(
                            pair.Value,
                            this.m_CategoryWordsDictionary.Sum(x => x.Value),
                            totalWords)));
            }

            

            foreach (Speech speech in m_CategorySpeeches)
            {
                foreach (KeyValuePair<string,int> Ngrams in m_NGramDictionary)
                {
                    this.m_NGramWordProbabilities.Add(new Probability(Ngrams.Key, Ngrams.Value, BayesianCalculator.WordProbability(Ngrams.Value, m_NGramDictionary.Sum(x => x.Value),totalNGrams)));
                }
            }
        }

        /// <summary>
        /// Calculates the term frequency inverse document frequency foreach word
        /// </summary>
        /// <param name="totalDocuments"></param>
        /// <param name="wordsToSpeeches"></param>
        public void CalculateTFIDF(int totalDocuments, Dictionary<Speech, List<string>> wordsToSpeeches)
        {
            foreach (Probability probability in this.m_WordProbabilities)
            {
                //Calculate the scripts contain the word
                int scriptsContainingWord = 0;
                foreach (KeyValuePair<Speech, List<string>> wordsToSpeechPair in wordsToSpeeches)
                {
                    foreach (string word in wordsToSpeechPair.Value)
                    {
                        if (word == probability.Word)
                        {
                            scriptsContainingWord++;
                            break;
                        }
                    }
                }

                probability.CalculateTFIDF(
                    this.m_CategoryWordsDictionary.Sum(x => x.Value),
                    totalDocuments,
                    scriptsContainingWord);
            }
        }
    }
}