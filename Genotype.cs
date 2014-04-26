using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    class Genotype
    {
        public int Length {get; set;}
        public EvolutionStrategy evolutionStrategy{get; set;}
        public List<double> Genes { get; set; }

        public Genotype(int length, EvolutionStrategy algorithm)
        {
            evolutionStrategy = algorithm;
            Length = length;

            Genes = GenerateGenes();
        }

        private List<double> GenerateGenes()
        {
            List<double> population = new List<double>();

            for(int i = 0; i < Length; i++)
            {
                //var tmpDoubleArray = new double[4];
                //evolutionStrategy.random.NextDouble();
                //doubleList.AddRange(tmpDoubleArray);
                population.Add(evolutionStrategy.random.NextDouble());
            }
            return population;
        }

        public void ShowGenes()
        {
            foreach (var val in Genes)
            {
                Console.WriteLine(val);
            }

        }

    }
}
