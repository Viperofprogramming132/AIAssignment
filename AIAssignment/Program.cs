// Project: AIAssignment
// Filename; Program.cs
// Created; 10/10/2018
// Edited: 11/10/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    class Program
    {

        static void Main(string[] args)
        {
            BayesianManager manager = new BayesianManager();
            char input;
            do
            {
                Console.Clear();
                Console.WriteLine("Please Select an Option:\n");
                Console.WriteLine("A) Undertake training");
                Console.WriteLine("B) Undertake Classification");
                Console.WriteLine("Q) Exit");

                char.TryParse(Console.ReadLine(), out input);

                input = Char.ToLower(input);
                switch (input)
                {
                    case 'a':
                        //TODO: Training
                        manager.StartTraining();
                        break;
                    case 'b':
                        //TODO: Classification
                        manager.ClassifyDocument();
                        break;
                    case 'q':
                        //TODO: Exit
                        break;
                    default:
                        Console.WriteLine("Please only input A, B or Q to exit (not case sensitive).");
                        break;
                }

            }
            while (input != 'q');
        }
    }
}