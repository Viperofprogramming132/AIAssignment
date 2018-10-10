using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    using System.IO;

    public class BayesainNetwork
    {
        private List<FileInfo> m_TrainingDataFiles;

        /// <summary>
        /// Starts the training process
        /// </summary>
        public void StartTraining()
        {
            this.m_TrainingDataFiles = new List<FileInfo>();
            this.GetTrainingData();
        }

        private void GetTrainingData()
        {
            bool badInput = false;
            bool retry = false;
            Console.Clear();

            //Get the directory and all the files inside of it
            DirectoryInfo trainingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\training");
            FileInfo[] possibleTrainingFiles = trainingDirectory.GetFiles("*.txt");

            //Write all the files inside of the directory
            Console.WriteLine("Possible files found: " + possibleTrainingFiles.Length + ".\n");
            for (int i = 0;i < possibleTrainingFiles.Length; ++i)
            {
                Console.WriteLine(i + 1 + ") " + possibleTrainingFiles[i]);
            }

            do
            {
                retry = false;
                badInput = false;
                Console.WriteLine("Please select which you wish to use (Separate selection with a comma ',').");

                string input = Console.ReadLine();

                string[] selections = input.Split(',');

                foreach (string selection in selections)
                {
                    //Make sure they only input numbers between the commas
                    try
                    {
                        this.m_TrainingDataFiles.Add(possibleTrainingFiles[int.Parse(selection) - 1]);
                    }
                    catch
                    {
                        badInput = true;
                    }
                }

                //Write out selected files
                Console.WriteLine("Selected files are:");
                foreach (FileInfo file in this.m_TrainingDataFiles)
                {
                    Console.WriteLine(file.Name);
                }

                //Ensure there is no bad inputs
                if (badInput)
                {
                    Console.WriteLine("One or more of the inputs were not added do you wish to try again (y/n)");
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
    }
}
