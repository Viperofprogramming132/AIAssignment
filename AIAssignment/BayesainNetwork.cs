// Project: AIAssignment
// Filename; BayesainNetwork.cs
// Created; 10/10/2018
// Edited: 16/10/2018

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment.Network
{
    /// <summary>
    /// Main control for the Bayesian Network
    /// </summary>
    public class BayesainNetwork
    {
        /// <summary>
        /// Political Party names
        /// </summary>
        private readonly string[] m_PartyNames = { "Coalition", "Conservative", "Labour" };

        /// <summary>
        /// Party names based off selected data
        /// </summary>
        private List<Category> m_Categories = new List<Category>();

        /// <summary>
        /// The Speech files and all they contain
        /// </summary>
        private List<Speech> m_TrainingDataFiles;

        /// <summary>
        /// The document that will be classified
        /// </summary>
        private Speech m_ClassifyDocument;

        /// <summary>
        /// The list of a list of probabilities that the words are found in both the category and the document to classify
        /// </summary>
        private Dictionary<Category, List<Probability>> m_WordsFoundInCategoryList =
            new Dictionary<Category, List<Probability>>();

        /// <summary>
        /// The list of a list of probabilities that the nGrams are found in both the category and the document to classify
        /// </summary>
        private Dictionary<Category, List<Probability>> m_NGramsFoundInCategoryList =
            new Dictionary<Category, List<Probability>>();

        /// <summary>
        /// The dictionary containing the category that the document was compared against with the probability of it being that category
        /// </summary>
        Dictionary<Category, double> m_CategoryProbabilities = new Dictionary<Category, double>();

        /// <summary>
        /// The dictionary containing the category that the document was compared against with the probability of it being that category that is calculated using nGrams
        /// </summary>
        Dictionary<Category, double> m_CategoryProbabilitiesWithNGrams = new Dictionary<Category, double>();

        /// <summary>
        /// Party names based off selected data
        /// </summary>
        public List<Category> Categories
        {
            get => this.m_Categories;
            set => this.m_Categories = value;
        }

        /// <summary>
        /// Gets all the .txt files in the given directory and outputs them to the console
        /// </summary>
        /// <param name="workingDirectory">The directory to look for the txt files</param>
        /// <returns>Array of all the files</returns>
        private FileInfo[] GetFilesInLocation(string workingDirectory)
        {
            //Get the directory and all the files inside of it
            DirectoryInfo trainingDirectory = new DirectoryInfo(workingDirectory);
            FileInfo[] files = trainingDirectory.GetFiles("*.txt");

            //Write all the files inside of the directory
            Console.WriteLine(@"Possible files found: " + files.Length + ".\n");
            for (int i = 0; i < files.Length; ++i)
            {
                Console.WriteLine(i + 1 + @") " + files[i]);
            }

            return files;
        }

        #region Training

        /// <summary>
        /// Starts the training process
        /// </summary>
        public void StartTraining()
        {
            this.m_TrainingDataFiles = new List<Speech>();

            this.GetTrainingData();
            this.SetCategories();
            this.CalculateCategoryProbabilities();
            this.CalculateWeightedProbabilities();
        }

        /// <summary>
        /// Reads the training files and asks the user which they wish to use
        /// </summary>
        private void GetTrainingData()
        {
            bool retry;
            Console.Clear();

            FileInfo[] possibleTrainingFiles = this.GetFilesInLocation(Directory.GetCurrentDirectory() + "\\training");

            do
            {
                bool badInput = false;
                retry = false;

                Console.WriteLine(
                    @"Please select which files you wish to use (Separate selection number with a comma ',').");

                string input = Console.ReadLine();

                //Ensure the string is not empty
                if (input != string.Empty)
                {
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
                }
                else
                {
                    badInput = true;
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
                    char.TryParse(Console.ReadLine(), out char cont);

                    cont = Char.ToLower(cont);

                    //Check if they wish to retry otherwise continue
                    if (cont == 'y') retry = true;
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
                        if (!this.Categories.Any(x => x.ToString() == name))
                        {
                            this.Categories.Add(new Category(name));
                            this.Categories.Last().GetCategorySpeeches().Add(file);
                        }
                        else
                        {
                            this.Categories[this.Categories.FindIndex(x => x.ToString() == name)].GetCategorySpeeches()
                                .Add(file);
                        }
                    }
                }
            }

            double errorCheck = 0;

            //Sets the probability of each category happening
            foreach (Category c in this.Categories)
            {
                c.Probability = BayesianCalculator.ProbabilityOfCategory(
                    c.GetCategorySpeeches().Count,
                    this.m_TrainingDataFiles.Count);

                errorCheck += c.Probability;
            }

            //If the total does not add to 1 then there is something wrong with the math
            if (Math.Abs(errorCheck - 1.0D) > 0.0000001)
            {
                Debug.WriteLine(@"Error: Probability does not add to 100%");
            }
        }

        /// <summary>
        /// Calculates P(word|category) and saves it to a list
        /// </summary>
        private void CalculateCategoryProbabilities()
        {
            //Used to work out total words/Ngrams
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
            Dictionary<string, int> nGramDictionary = new Dictionary<string, int>();

            foreach (Category category in this.Categories)
            {
                //Makes the category make its speeches word dictionary one dictionary.
                category.CondenseSpeeches();

                //Get all words to calculate total
                foreach (KeyValuePair<string, int> pair in category.GetCategoryWordsDictionary())
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

                //Get all Ngrams to calculate total
                foreach (KeyValuePair<string, int> pair in category.GetCategoryNGramDictionary())
                {
                    if (nGramDictionary.ContainsKey(pair.Key))
                    {
                        nGramDictionary[pair.Key] += pair.Value;
                    }
                    else
                    {
                        nGramDictionary.Add(pair.Key, pair.Value);
                    }
                }
            }
            
            //Calculates P(word|category)
            foreach (Category category in this.Categories)
            {
                category.CalculateWordProb(wordsDictionary.Count,nGramDictionary.Count);
            }
        }

        /// <summary>
        /// Calculates the term frequency inverse document frequency foreach of the words in every category
        /// </summary>
        private void CalculateWeightedProbabilities()
        {
            int totalDocuments = 0;
            Dictionary<Speech, List<string>> wordsToSpeeches = new Dictionary<Speech, List<string>>();
            Dictionary<Speech, List<string>> nGramsToSpeeches = new Dictionary<Speech, List<string>>();

            foreach (Category category in this.m_Categories)
            {
                totalDocuments += category.GetCategorySpeeches().Count;
                foreach (Speech speech in category.GetCategorySpeeches())
                {
                    //Get all the contained words in every speech
                    List<string> containedWords = new List<string>();
                    foreach (KeyValuePair<string, int> words in speech.WordsDictionary)
                    {
                        containedWords.Add(words.Key);
                    }

                    //Get all the contained nGrams in every speech
                    List<string> containedNGrams = new List<string>();
                    foreach (KeyValuePair<string, int> words in speech.NGramDictionary)
                    {
                        containedNGrams.Add(words.Key);
                    }

                    nGramsToSpeeches.Add(speech, containedNGrams);
                    wordsToSpeeches.Add(speech, containedWords);
                }
            }

            foreach (Category category in this.m_Categories)
            {
                category.CalculateTFIDF(totalDocuments, wordsToSpeeches, nGramsToSpeeches);
            }
        }

        #endregion

        #region Classify

        /// <summary>
        /// Starts the classification of an unknown document
        /// </summary>
        public void ClassifyDocument()
        {
            //Ensure nothing is taken from the last classification
            this.m_WordsFoundInCategoryList = new Dictionary<Category, List<Probability>>();
            this.m_CategoryProbabilities = new Dictionary<Category, double>();
            this.m_ClassifyDocument = null;
            this.m_NGramsFoundInCategoryList = new Dictionary<Category, List<Probability>>();
            this.m_CategoryProbabilitiesWithNGrams = new Dictionary<Category, double>();

            this.SelectDocument();
            this.FindExistingWords();
            this.GetProbabilities();
            this.OutputResult();
        }


        /// <summary>
        /// Gets the documents from the test folder and asks the user which one they wish to use
        /// </summary>
        private void SelectDocument()
        {
            FileInfo[] possibleTestFiles = this.GetFilesInLocation(Directory.GetCurrentDirectory() + "\\test");

            bool retry;

            do
            {
                bool badInput = false;
                retry = false;

                Console.WriteLine(@"Please select which file you wish to use.");

                string input = Console.ReadLine();

                //ensures the input was good
                try
                {
                    this.m_ClassifyDocument = new Speech(possibleTestFiles[int.Parse(input) - 1]);
                }
                catch
                {
                    badInput = true;
                }

                if (!badInput)
                {
                    //Write out selected files
                    Console.WriteLine(@"Selected file is:");
                    Console.WriteLine(this.m_ClassifyDocument.FileInf.Name);
                }

                //Ensure there is no bad inputs
                if (badInput)
                {
                    Console.WriteLine(@"The input was not added do you wish to try again (y/n)");
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
        /// Finds all the common words between the categories and the document to classify
        /// </summary>
        private void FindExistingWords()
        {
            foreach (Category category in this.Categories)
            {
                //Find the common words between the category documents and the one to classify
                List<Probability> commonWordList = new List<Probability>();

                foreach (KeyValuePair<string, int> wordPair in this.m_ClassifyDocument.WordsDictionary)
                {
                    foreach (Probability cateWord in category.WordProbabilities)
                    {
                        if (wordPair.Key == cateWord.Word)
                        {
                            commonWordList.Add(cateWord);
                        }
                    }
                }

                this.m_WordsFoundInCategoryList.Add(category, commonWordList);

                //Find the common nGrams between the category documents and the one to classify
                List<Probability> commonNGramList = new List<Probability>();

                foreach (KeyValuePair<string, int> wordPair in this.m_ClassifyDocument.NGramDictionary)
                {
                    foreach (Probability cateWord in category.NGramWordProbabilities)
                    {
                        if (wordPair.Key == cateWord.Word)
                        {
                            commonNGramList.Add(cateWord);
                        }
                    }
                }

                this.m_NGramsFoundInCategoryList.Add(category, commonNGramList);
            }
        }

        /// <summary>
        /// Calculates the probability of the document being in each category
        /// </summary>
        private void GetProbabilities()
        {
            //Calculate the probabilities of the document being in each category by using word TFIDF probability
            foreach (KeyValuePair<Category, List<Probability>> categoryWordPair in this.m_WordsFoundInCategoryList)
            {
                this.m_CategoryProbabilities.Add(
                    categoryWordPair.Key,
                    BayesianCalculator.DocumentProbability(categoryWordPair.Value, categoryWordPair.Key.Probability));
            }

            //Calculate the probabilities of the document being in each category by using word nGram and TFIDF probability
            foreach (KeyValuePair<Category, List<Probability>> categoryNGramPair in this.m_NGramsFoundInCategoryList)
            {
                this.m_CategoryProbabilitiesWithNGrams.Add(
                    categoryNGramPair.Key,
                    BayesianCalculator.DocumentProbability(categoryNGramPair.Value, categoryNGramPair.Key.Probability));
            }
        }

        /// <summary>
        /// Outputs the result to the console and to results.txt for easy access
        /// </summary>
        private void OutputResult()
        {
            List<string> outputs = new List<string>();

            //Gets the smallest number due to larger probability being a larger negative due to logarithms
            int indexOfLargest = this.m_CategoryProbabilities.Values.ToList()
                .IndexOf(this.m_CategoryProbabilities.Values.Min());

            //Add the normal calculation to the results using TFIDF
            outputs.Add("\nThe file " + this.m_ClassifyDocument.FileInf.Name + @" is classified under the " + this.m_CategoryProbabilities.Keys.ToList()[indexOfLargest] + @" category using TFIDF");

            //Do it again for using nGrams
            indexOfLargest = this.m_CategoryProbabilitiesWithNGrams.Values.ToList()
                .IndexOf(this.m_CategoryProbabilitiesWithNGrams.Values.Min());

            outputs.Add("\nThe file " + this.m_ClassifyDocument.FileInf.Name + @" is classified under the " + this.m_CategoryProbabilities.Keys.ToList()[indexOfLargest] + " category using nGrams and TFIDF\n\n");

            //Output results
            foreach (string output in outputs)
            {
                Console.WriteLine(output);
            }

            //Display probability in percentage for TFIDF probabilities
            foreach (KeyValuePair<Category,double> percentageKeyValuePair in this.CalculatePercentage(this.m_CategoryProbabilities))
            {
                Console.WriteLine("Probability of " + percentageKeyValuePair.Key + " is " + percentageKeyValuePair.Value + " using TFIDF");
            }

            //Display probability in percentage for nGram and TFIDF probabilities
            foreach (KeyValuePair<Category, double> percentageKeyValuePair in this.CalculatePercentage(this.m_CategoryProbabilitiesWithNGrams))
            {
                Console.WriteLine("Probability of " + percentageKeyValuePair.Key + " is " + percentageKeyValuePair.Value + " using nGrams and TFIDF");
            }

            //Write it to a file so it can be checked later
            File.AppendAllLines("results.txt", outputs);
            Console.ReadKey();
        }


        /// <summary>
        /// Calculates the percentage probability of the categories occurring for displaying
        /// </summary>
        /// <param name="probabilityDictionary"></param>
        /// <returns>The Category paired with a double which is the percentage over all the given doubles</returns>
        private Dictionary<Category, double> CalculatePercentage(Dictionary<Category,double> probabilityDictionary)
        {
            Dictionary<Category, double> categoryPercentageDictionary = new Dictionary<Category, double>();
            //Sum them to divide by
            double total = probabilityDictionary.Sum(x => x.Value);
            //Make sure they are positive as they default to negative
            total = total * -1;
            foreach (KeyValuePair<Category,double> categoryProbabilityPair in probabilityDictionary)
            {
                categoryPercentageDictionary.Add(
                    categoryProbabilityPair.Key,
                    ((categoryProbabilityPair.Value * -1) / total)*100);
            }

            double errorCheck = categoryPercentageDictionary.Sum(x => x.Value);

            if (Math.Abs(errorCheck - 100.0D) > 0.1)
            {
                Debug.WriteLine("Percentages do not add to 100%");
            }

            return categoryPercentageDictionary;
        }

        #endregion
    }
}
