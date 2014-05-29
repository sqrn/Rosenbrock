using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    /// <summary>
    /// klasa służąca do porównywania genotypów po wartości funkcji
    /// </summary>
    class GenotypeComparer : IComparer<Genotype>
    {
        public GenotypeComparer()
        {
        }

        public int Compare(Genotype x, Genotype y)
        {
            if (x.FunctionValue > y.FunctionValue)
                return 1;
            else if (x.FunctionValue < y.FunctionValue)
                return -1;
            else
                return 0;
        }
    }
}