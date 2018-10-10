// Project: AIAssignment
// Filename; BayesainNetwork.cs
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

    public class BayesainNetwork
    {
        /// <summary>
        /// Political Party names
        /// </summary>
        private readonly string[] m_PartyNames = { "Coalition", "Conservative", "Labour" };

        /// <summary>
        /// The Speech files and all they contain
        /// </summary>
        private List<Speech> m_TrainingDataFiles;

        /// <summary>
        /// Party names based off selected data
        /// </summary>
        private readonly List<Category> m_Categories = new List<Category>();

        /// <summary>
        /// Starts the training process
        /// </summary>
        public void StartTraining()
        {
            this.m_TrainingDataFiles = new List<Speech>();
            this.GetTrainingData();
            this.SetCategories();
            this.CalculateCategoryProbabilities();
        }

        /// <summary>
        /// Reads the training files and asks the user which they wish to use
        /// </summary>
        private void GetTrainingData()
        {
            bool retry;
            Console.Clear();

            //Get the directory and all the files inside of it
            DirectoryInfo trainingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\training");
            FileInfo[] possibleTrainingFiles = trainingDirectory.GetFiles("*.txt");

            //Write all the files inside of the directory
            Console.WriteLine(@"Possible files found: " + possibleTrainingFiles.Length + ".\n");
            for (int i = 0; i < possibleTrainingFiles.Length; ++i)
            {
                Console.WriteLine(i + 1 + @") " + possibleTrainingFiles[i]);
            }

            do
            {
                bool badInput = false;
                retry = false;

                Console.WriteLine(@"Please select which you wish to use (Separate selection with a comma ',').");

                string input = Console.ReadLine();

                string[] selections = input.Split(',');

                foreach (string selection in selections)
                {
                    //Make sure they only input numbers between the commas
                    try
                    {
                        this.m_TrainingDataFiles.Add(new Speech(possibleTrainingFiles[int.Parse(selection) - 1]));
                    }
                    catch
                    {
                        badInput = true;
                    }
                }

                //Write out selected files
                Console.WriteLine(@"Selected files are:");
                foreach (Speech file in this.m_TrainingDataFiles)
                {
                    Console.WriteLine(file.FileInf.Name);
                }

                //Ensure there is no bad inputs
                if (badInput)
                {
                    Console.WriteLine(@"One or more of the inputs were not added do you wish to try again (y/n)");
                    char cont = char.Parse(Console.ReadLine());

                    cont = Char.ToLower(cont);

                    //Check if they wish to retry otherwise continue
                    switch (cont)
                    {
                        case 'y':
                            retry = true;
                            break;
                    }
                }
            }
            while (retry);
        }

        /// <summary>
        /// Sets the categories for the speeches and adds them to list
        /// </summary>
        private void SetCategories()
        {
            foreach (Speech file in this.m_TrainingDataFiles)
            {
                foreach (string name in this.m_PartyNames)
                {
                    if (file.FileInf.Name.Contains(name))
                    {
                        //Check if the category already exists
                        if (!this.m_Categories.Any(x => x.ToString() == name))
                        {
                            this.m_Categories.Add(new Category(name));
                            this.m_Categories.Last().CategorySpeeches.Add(file);
                        }
                        else
                        {
                            this.m_Categories[this.m_Categories.FindIndex(x => x.ToString() == name)].CategorySpeeches
                                .Add(file);
                        }
                    }
                }
            }

            double errorCheck = 0;

            //Sets the probability of each category happening
            foreach (Category c in this.m_Categories)
            {
                c.Probability = BayesianCalculator.ProbabilityOfCategory(
                    c.CategorySpeeches.Count,
                    this.m_TrainingDataFiles.Count);

                errorCheck += c.Probability;
            }

            //If the total does not add to 1 then there is something wrong with the math
            if (errorCheck != 1.0D)
            {
                Console.WriteLine(@"Error: Probability does not add to 100%");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Calculates P(word|category) and saves it to a list
        /// </summary>
        private void CalculateCategoryProbabilities()
        {
            //Used to work out total words
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();

            foreach (Category category in this.m_Categories)
            {
                //Makes the category make its speeches word dictionary one dictionary.
                category.CondenseSpeeches();

                //Get all words to calculate total
                foreach (KeyValuePair<string, int> pair in category.CategoryWordsDictionary)
                {
                    if (wordsDictionary.ContainsKey(pair.Key))
                    {
                        wordsDictionary[pair.Key] += pair.Value;
                    }
                    else
                    {
                        wordsDictionary.Add(pair.Key, pair.Value);
                    }
                }
            }

            //Calculates P(word|category)
            foreach (Category category in this.m_Categories)
            {
                category.CalculateWordProb(wordsDictionary.Count);
            }
        }
    }
}