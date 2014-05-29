using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    class Genotype
    {
        public int Length {get; set;}
        public EvolutionStrategy EvolutionStrategy{get; set;}
        public Double FunctionValue { get; set; }
        public double[] Genes { get; set; }


        public Genotype(int length, EvolutionStrategy algorithm)
        {
            EvolutionStrategy = algorithm;
            Length = length;

            Genes = GenerateGenes();
        }

        /// <summary>
        /// Funkcja generuje geny
        /// </summary>
        /// <returns></returns>
        private double[] GenerateGenes()
        {
            var genesArray = new double[Length];
            for(int i = 0; i < Length; i++)
            {
                //var rnd = EvolutionStrategy.Random.NextDouble();
                var rnd = EvolutionStrategy.Random.Next(-20,20);
                genesArray[i] = rnd;
            }
            return genesArray;
        }

        /// <summary>
        /// Tutaj modyfikujemy geny rodzicielskich chromosomów
        /// </summary>
        /// <param name="parent2"></param>
        /// <param name="parent2"></param>
        /// <returns>2 child array</returns>
        public static Genotype[] Crossover(Genotype parent1, Genotype parent2)
        {
            if (parent1.Length != parent2.Length) throw new Exception("Nie można krzyżować osobników z różną długością!");
            var rnd = parent1.EvolutionStrategy.Random.NextDouble();//zmienna losowa z zakresu 0..1
            var length = parent1.Length;            
            var child1 = new Genotype(length, parent1.EvolutionStrategy);
            var child2 = new Genotype(length, parent1.EvolutionStrategy);


            for (int i = 0; i < length; i++)
            {
                child1.Genes[i] = parent1.Genes[i] + rnd * (parent2.Genes[i] - parent1.Genes[i]);
                child2.Genes[i] = parent2.Genes[i] + parent1.Genes[i] - child1.Genes[i];
            }

            return new Genotype[2] { child1, child2 };

        }

        /// <summary>
        /// Funkcja mutuje genotyp osobnika przez dodanie do wartości genu zmiennej losowej z rozkładu normanego Gaussa.
        /// </summary>
        public void Mutate(float mutationRate)
        {
            var gaussian = new GaussianGenerator();
            var normal = gaussian.NextDouble(0,1);

            for(int i=0; i < Length; i++) 
            {
                if(EvolutionStrategy.Random.NextDouble() < mutationRate)
                    this.Genes[i] = this.Genes[i] + normal;
            }
        }

        /// <summary>
        /// Metoda konwertuje bajty na reprezentacje rzeczywistoliczbowa i zwraca tablice
        /// wartosci punktów funkcji.
        /// Zgodnie z ksiażką Zbigniewa Michalewicza, strona 45.
        /// </summary>
        /// <returns>Tablica wartości wymiarów funkcji</returns>
        public double[] GetValues()
        {
            var genesArray = new double[Length];
            for (int i = 0; i < Length; i++)
            {
                genesArray[i] = this.Genes[i];
            }
            return genesArray;
        }

        public Genotype Copy()
        {
            var copyGenotype = new Genotype(this.Length, this.EvolutionStrategy);
            copyGenotype.Genes = this.Genes;
            return copyGenotype;
        }
    }
}
