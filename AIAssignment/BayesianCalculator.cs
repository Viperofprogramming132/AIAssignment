using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssignment
{
    public static class BayesianCalculator
    {
        public static double ProbabilityOfCategory(int NumOfCategory, int TotalDocs)
        {
            return (double)NumOfCategory / TotalDocs;
        }

        public static double WordProbability(int NumOfWord, int TotalCate, int TotalWords)
        {
            return (double)(NumOfWord + 1) / (TotalCate + TotalWords);
        }
    }
}
