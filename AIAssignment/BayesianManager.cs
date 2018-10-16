// Project: AIAssignment
// Filename; BayesianManager.cs
// Created; 10/10/2018
// Edited: 16/10/2018

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using AIAssignment.Network;

namespace AIAssignment
{
    /// <summary>
    /// Manages the network so it can be serialized easier
    /// </summary>
    public class BayesianManager
    {
        /// <summary>
        /// The bayesain network that will be trained
        /// </summary>
        private BayesainNetwork m_Network;

        /// <summary>
        /// Background worker for serialization
        /// </summary>
        private BackgroundWorker m_SerializerBackgroundWorker;

        /// <summary>
        /// Starts the training process upon completion serializes the network and starts the classification process
        /// </summary>
        public void StartTraining()
        {
            this.m_SerializerBackgroundWorker = new BackgroundWorker();
            this.m_SerializerBackgroundWorker.DoWork += this.SerializeNetworkAsync;

            this.m_Network = null;
            this.m_Network = new BayesainNetwork();

            //Actually start training
            this.m_Network.StartTraining();

            //Serialize network
            this.m_SerializerBackgroundWorker.RunWorkerAsync();

            Console.Clear();
            Console.WriteLine(@"Continuing to Classification");
            Console.ReadKey();

            //Continue to classification
            this.ClassifyDocument();
        }

        /// <summary>
        /// Starts the process of classifying and unknown document
        /// </summary>
        public void ClassifyDocument()
        {
            Console.Clear();

            //Check if the user wants to read in the save file if they do not start the training process
            if (this.m_Network == null)
            {
                Console.WriteLine(
                    @"Do you wish to read the data from the saved file (y/n) (if no training will commence)");

                string input = Console.ReadLine();

                if (input != string.Empty)
                {
                    input = input.ToLower();

                    if (input[0] == 'y')
                    {
                        this.DeserializeNetwork();
                    }
                    else
                    {
                        this.StartTraining();
                    }
                }
                else
                {
                    Console.WriteLine(@"Invalid input. Starting training");
                    Console.ReadKey();
                    this.StartTraining();
                }
            }

            this.m_Network.ClassifyDocument();
        }

        /// <summary>
        /// Serializes the network and all the probabilities to save.xml
        /// Background Serializer function done with background worker to reduce freezing
        /// </summary>
        /// <param name="sender">Contains the background worker that called the function</param>
        /// <param name="e">The arguments of the Do work event mainly cancel</param>
        private void SerializeNetworkAsync(object sender, DoWorkEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BayesainNetwork));
            using (XmlWriter writer = new XmlTextWriter("save.xml", Encoding.UTF8))
            {
                serializer.Serialize(writer, this.m_Network);
            }
        }

        /// <summary>
        /// Deserializes the network form save.xml
        /// </summary>
        private void DeserializeNetwork()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\save.xml"))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(BayesainNetwork));
                    using (Stream stream = new FileStream("save.xml", FileMode.Open))
                    using (XmlReader reader = new XmlTextReader(stream))
                    {
                        this.m_Network = (BayesainNetwork)serializer.Deserialize(reader);
                    }
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine(@"File not found. Starting training instead");
                    this.StartTraining();
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine(@"File not found. Starting training instead");
                this.StartTraining();
            }
        }
    }
}