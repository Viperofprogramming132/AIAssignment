
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIAssignment
{
    public class Speech
    {
        /// <summary>
        /// Contains all the characters to remove from the speeches
        /// </summary>
        private static readonly char[] m_NonVerbalContent = { '"', ':', ';', '\n', '\t', '.', ',' };

        /// <summary>
        /// Contains all the stopwords to remove from the speech
        /// </summary>
        private readonly string[] m_Stopwords;

        /// <summary>
        /// Contains all information about the file
        /// </summary>
        private FileInfo m_File;

        /// <summary>
        /// The Speech as it is before any editing
        /// </summary>
        private string m_SpeechScript;

        /// <summary>
        /// The words and how often they occur
        /// </summary>
        Dictionary<string,int> m_WordsDictionary = new Dictionary<string, int>();

        /// <summary>
        /// Constructor for the Speech gets all the information from the files
        /// </summary>
        /// <param name="file"></param>
        public Speech(FileInfo file)
        {
            this.m_File = file;
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
                        speech.RemoveAt(i);
                        i--;
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
            string[] words = this.m_SpeechScript.ToLower().Split(' ');

            foreach (string word in words)
            {
                //Checks if the word is not a stopword
                if (!this.m_Stopwords.Any(x => x == word))
                {
                    if (this.m_WordsDictionary.ContainsKey(word))
                    {
                        this.m_WordsDictionary[word]++;
                    }
                    else
                    {
                        this.m_WordsDictionary.Add(word, 1);
                    }
                }
            }
        }
    }
}
