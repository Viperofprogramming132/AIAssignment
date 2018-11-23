// Project: AIAssignment
// Filename; Speech.cs
// Created; 10/10/2018
// Edited: 17/11/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment.Network
{
    public class Speech
    {
        /// <summary>
        /// Contains all the characters to remove from the speeches
        /// </summary>
        private static readonly char[] m_NonVerbalContent = { '"', ':', ';', '\n', '\t', '.', ',', '\r' };

        /// <summary>
        /// Contains all the stopwords to remove from the speech
        /// </summary>
        private readonly string[] m_Stopwords;

        /// <summary>
        /// Contains all information about the file
        /// </summary>
        private readonly FileInfo m_File;

        /// <summary>
        /// The Speech as it is before any editing
        /// </summary>
        private string m_SpeechScript;

        /// <summary>
        /// The words and how often they occur
        /// </summary>
        private readonly Dictionary<string, int> m_WordsDictionary = new Dictionary<string, int>();

        /// <summary>
        /// The nGram words and how often they occur
        /// </summary>
        private Dictionary<string, int> m_NGramDictionary = new Dictionary<string, int>();

        /// <summary>
        /// The script file name
        /// </summary>
        private string m_FileName;

        /// <summary>
        /// Constructor for the Speech gets all the information from the files
        /// </summary>
        /// <param name="file"></param>
        public Speech(FileInfo file)
        {
            this.m_File = file;
            this.m_FileName = this.m_File.FullName;
            this.m_Stopwords = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\stopwords.txt");
            this.GetScript();
            this.FillDictionary();
        }

        /// <summary>
        /// Empty constructor for serialization 
        /// </summary>
        public Speech()
        {
            this.m_File = new FileInfo(this.m_FileName);
            this.m_Stopwords = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\stopwords.txt");
            this.GetScript();
            this.FillDictionary();
        }

        /// <summary>
        /// Returns the File Information
        /// </summary>
        public FileInfo FileInf
        {
            get => this.m_File;
        }

        /// <summary>
        /// Returns the Words and how often they occur
        /// </summary>
        public Dictionary<string, int> WordsDictionary
        {
            get => this.m_WordsDictionary;
        }


        /// <summary>
        /// Get and set for the N-Gram Dictionary
        /// </summary>
        public Dictionary<string, int> NGramDictionary
        {
            get => this.m_NGramDictionary;
            set => this.m_NGramDictionary = value;
        }

        /// <summary>
        /// Gets the speech from the file and removes all non verbal content
        /// </summary>
        private void GetScript()
        {
            //Read the script in from file
            this.m_SpeechScript = File.ReadAllText(this.m_File.FullName);

            //Convert it to a Char array for easier manipulation
            List<char> speech = this.m_SpeechScript.ToCharArray().ToList();

            //Remove all non verbal content
            for (int i = 0; i < speech.Count; i++)
            {
                foreach (char c in m_NonVerbalContent)
                {
                    if (speech[i] == c)
                    {
                        if (c == '\n')
                        {
                            speech[i] = ' ';
                        }
                        else
                        {
                            speech.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            this.m_SpeechScript = new string(speech.ToArray());
        }

        /// <summary>
        /// Fills the dictionary of the non stopword words
        /// </summary>
        private void FillDictionary()
        {
            Stemmer stemmer = new Stemmer();
            string[] words = this.m_SpeechScript.ToLower().Split(' ');
            words = this.RemoveStopwords(words).ToArray();

            foreach (string word in words)
            {
                //Checks if the word is not a stopword
                string stemmedWord = stemmer.StemWord(word);
                if (this.m_WordsDictionary.ContainsKey(stemmedWord))
                {
                    this.m_WordsDictionary[stemmedWord]++;
                }
                else
                {
                    this.m_WordsDictionary.Add(stemmedWord, 1);
                }
            }
        }

        /// <summary>
        /// Removes all the stopwords from the script
        /// </summary>
        /// <param name="words">Array of all the words split up</param>
        /// <returns>IEnumerable containing all the words that are not contained within the stopwords</returns>
        private IEnumerable<string> RemoveStopwords(string[] words)
        {
            List<string> wordsList = new List<string>();

            foreach (string word in words)
            {
                if (this.m_Stopwords.All(x => x != word))
                {
                    wordsList.Add(word);
                }
            }

            return wordsList.AsEnumerable();
        }

        /// <summary>
        /// Async task creating the n-gram dictionary from the script
        /// </summary>
        /// <returns>The Task that is running the function</returns>
        public async Task FillNgrams()
        {
            string[] words = this.m_SpeechScript.Split(' ');
            string[] wordArray = this.RemoveStopwords(words).ToArray();
            this.m_NGramDictionary = NGram.CreateNGramFromScript(wordArray);
        }
    }
}