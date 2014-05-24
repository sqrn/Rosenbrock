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
        public double[] GenesInDouble { get; set; }

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

        /// <summary>
        /// Tutaj modyfikujemy geny rodzicielskich chromosomów
        /// </summary>
        /// <param name="parent2"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public Genotype[] Crossover(Genotype parent2)
        {
            var copyArray = new BitArray(Length * 32);
            var rnd = EvolutionStrategy.Random.NextDouble();//zmienna losowa z zakresu 0..1
            
            var child1 = new Genotype(Length, EvolutionStrategy);
            var child2 = new Genotype(Length, EvolutionStrategy);
            //konwersja wartosci chromosomow do liczby rzeczywistej
            //dodanie rnd do kazdego genu rodzica
            //powrotna konwersja na Bity

            var parent1values = this.GetValues();//tablica postaci rzeczywistej domyslnie [x,y]
            var parent2values = parent2.GetValues();

            for (int i = 0; i < Length; i++)
            {
                child1.GenesInDouble[i] = parent1values[i] + rnd * (parent2values[i] - parent1values[i]);
                child2.GenesInDouble[i] = parent2values[i] + parent1values[i] - child1.GenesInDouble[i];
            }
            return new Genotype[2] { child1, child2 };
        }

        public void Mutate()
        {
            var rnd = EvolutionStrategy.Random.NextDouble();//zmienna losowa z zakresu 0..1
            for(int i=0; i<Length; i++) 
            {
                this.GenesInDouble[i] = this.GenesInDouble[i] + rnd;
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
            var doubleNumbers = new double[Length];
            var bytearray = new byte[4 * Length];
            Genes.CopyTo(bytearray, 0);
            var power = Math.Pow(Length, 32);
            for (int i = 0; i < Length; i++)
            {
                var singleByteValue = bytearray.Skip(i * Length).Take(4).ToArray(); //konwersja na bajty
                var rawValue = BitConverter.ToUInt32(singleByteValue, 0); //konwersja na Uint
                //Console.WriteLine("Liczba: " + rawValue);
                doubleNumbers[i] = (rawValue * 10.0) / power; //tablica[Length] liczb rzeczywistych, default: Length=2
                //Console.WriteLine("Wrzucam -> " + doubleNumbers[i] );
            }
            GenesInDouble = doubleNumbers;
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

        public Genotype Copy()
        {
            var copyGenotype = new Genotype(this.Length, this.EvolutionStrategy);
            copyGenotype.Genes = this.Genes;
            return copyGenotype;
        }

        public double RankIndividual()
        {
            return this.GetValues().Sum();
        }

    }
}
