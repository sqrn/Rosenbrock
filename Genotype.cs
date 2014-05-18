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
        public BitArray Genes { get; set; }
        public int Length {get; set;}
        public EvolutionStrategy EvolutionStrategy{get; set;}
        public Double FunctionValue { get; set; }

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
        private BitArray GenerateGenes()
        {
            var byteList = new List<byte>();
            for(int i = 0; i < Length; i++)
            {
                var tmpByteArray = new byte[4];//32bity
                EvolutionStrategy.Random.NextBytes(tmpByteArray);
                byteList.AddRange(tmpByteArray);
            }
            return new BitArray(byteList.ToArray());
        }


        public Genotype[] Crossover(Genotype parent2, float crossoverRate)
        {
            Console.WriteLine("Krzyzowanie");
            var child1 = new Genotype(Length, EvolutionStrategy);
            var child2 = new Genotype(Length, EvolutionStrategy);

            return new Genotype[2] {child1, child2};
        }

        public void Mutate(float mutationRate)
        {
            Console.WriteLine("Mutacja");
        }

        /// <summary>
        /// Metoda konwertuje bajty na reprezentacje rzeczywistoliczbowa i zwraca tablice
        /// wartosci punktów funkcji.
        /// Zgodnie z ksiażką Zbigniewa Michalewicza, strona 45.
        /// </summary>
        /// <returns>Tablica wartości wymiarów funkcji</returns>
        public double[] GetValues()
        {
            var doubleNumbers = new double[Length];
            var bytearray = new byte[4 * Length];
            Genes.CopyTo(bytearray, 0);
            for (int i = 0; i < Length; i++)
            {
                var singleByteValue = bytearray.Skip(i * Length).Take(4).ToArray(); //konwersja na bajty
                var rawValue = BitConverter.ToUInt32(singleByteValue, 0); //konwersja na Uint
                doubleNumbers[i] = (0 + (rawValue * Length) / Math.Pow(Length, 32));//tablica[Length] liczb rzeczywistych, default: Length=2
            }
            return doubleNumbers;
        }

        public List<Genotype> CopyOf(int howMany)
        {
            List<Genotype> tmpList = new List<Genotype>();
            for (int i = 0; i < howMany; i++)
            {
                tmpList.Add(this);
            }
            return tmpList;
        }

    }
}
