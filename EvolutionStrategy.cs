using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    class EvolutionStrategy
    {
        public Random random;
        public int populationSize { get; set; }
        public int numberOfGenerations { get; set; }

        public List<Genotype> generation;


        public EvolutionStrategy(int populationSize, int numberOfGenerations)
        {
            if (populationSize % 2 != 0) throw new ArgumentException("Population size must be even!");

            
            this.populationSize = populationSize;
            this.numberOfGenerations = numberOfGenerations;

            random = new Random();
        }

        public void Run()
        {
            generation = new List<Genotype>(populationSize);

            for (int i = 0; i > numberOfGenerations; i++)
            { 
                ///
            }

        }
    }
}
