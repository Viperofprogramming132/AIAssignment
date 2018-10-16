// Project: AIAssignment
// Filename; Program.cs
// Created; 10/10/2018
// Edited: 16/10/2018

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
                Console.WriteLine(@"Please Select an Option:\n");
                Console.WriteLine(@"1) Undertake training");
                Console.WriteLine(@"2) Undertake Classification");
                Console.WriteLine(@"Q) Exit");

                char.TryParse(Console.ReadLine(), out input);

                input = char.ToLower(input);
                switch (input)
                {
                    case '1':
                        manager.StartTraining();
                        break;
                    case '2':
                        manager.ClassifyDocument();
                        break;
                    case 'q':
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