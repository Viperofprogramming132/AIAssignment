// Project: AIAssignment
// Filename; BayesianManager.cs
// Created; 10/10/2018
// Edited: 11/10/2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AIAssignment
{
    public class BayesianManager
    {
        private BayesainNetwork m_Network;
        public BayesianManager()
        {

        }

        public void SerializeNetwork()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BayesainNetwork));
            using (XmlWriter writer = new XmlTextWriter("save.xml", Encoding.UTF8))
            {
                serializer.Serialize(writer, this.m_Network);
            }
            
        }

        private void DeserializeNetwork()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BayesainNetwork));
            using (Stream stream = new FileStream("save.xml", FileMode.Open))
            using (XmlReader reader = new XmlTextReader(stream))
            {
                this.m_Network = (BayesainNetwork)serializer.Deserialize(reader);
            }
        }

        public void StartTraining()
        {
            this.m_Network = null;
            GC.Collect();
            this.m_Network = new BayesainNetwork();
            this.m_Network.StartTraining();

            this.SerializeNetwork();

            Console.Clear();
            Console.WriteLine(@"Continuing to Classification");
            Console.ReadKey();

            this.ClassifyDocument();
        }

        public void ClassifyDocument()
        {
            if (this.m_Network == null)
            {
                Console.WriteLine(@"Do you wish to read the data from the saved file (y/n) (if no training will commence)");

                string input = Console.ReadLine();

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

            this.m_Network.ClassifyDocument();
        }
    }
}