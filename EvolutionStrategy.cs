using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    class EvolutionStrategy
    {
        public Random Random;
        public int PopulationSize { get; set; }
        public int SigmaPopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        public int GenotypeSize { get; set; }

        public Genotype BestGenotype { get; set; }
        public int BestGenotypeGeneration { get; set; }


        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;

        public EvolutionStrategy(int populationSize, int sigmaPopulationSize, int numberOfGenerations, int genotypeSize, FunctionToOptimize function)
        {
            if (PopulationSize % 2 != 0) throw new ArgumentException("Population size must be even!");

            PopulationSize = populationSize;
            SigmaPopulationSize = sigmaPopulationSize;
            NumberOfGenerations = numberOfGenerations; 
            GenotypeSize = genotypeSize;

            Random = new Random();
        }

        public void Run()
        {
            ThisGeneration = new List<Genotype>(PopulationSize);//obecna generacja
            NextGeneration = new List<Genotype>(SigmaPopulationSize);//potomkowie
            BestGenotype = null;

            CreateFirstGeneration(); //utworzenie pierwszej generacji osobnikow
            //RankPopulation(ref ThisGeneration);
            for (int i = 0; i < NumberOfGenerations; i++)
            {
                NextGeneration = Reproduction();
                Console.WriteLine("NextGeneration size: " + NextGeneration.Count());
            }

        }

        public void ShowPopulation()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Console.WriteLine(ThisGeneration[i].GetValues()[0] + "; " + ThisGeneration[i].GetValues()[1]);
            }
        }

        public List<Genotype> Reproduction()
        {
            NextGeneration = DeterministicSelection(ref ThisGeneration);
            return NextGeneration;
            //returnGeneration = Crossover(generation);
        }
        /// <summary>
        /// Selekcja deterministyczna.
        /// Na podstawie http://www.bialystok.edu.pl/cen/archiwum/mat_dyd/Informatyk/sztuczna_int.htm
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Genotype> DeterministicSelection(ref List<Genotype> generation)
        {
            //Console.WriteLine("Rozpoczynam proces deterministycznej selekcji osobnikow...");
            var tmpNextGeneration = new List<Genotype>(SigmaPopulationSize);//potomkowie
            var tmpAdaptationList = new List<double>();
            var adaptationOfIndividualList = new List<int>();
            double hi; //srednie przystosowanie osobnika
            double hs; //srednie przystosowanie calej populacji
            double ki; //oczekiwana liczba kopii osobnika

            foreach (Genotype individual in generation)
            {
                hi = individual.GetValues().Sum();
                tmpAdaptationList.Add(hi);
            }
            hs = tmpAdaptationList.Sum() / PopulationSize;
            Console.WriteLine("HS: " + hs);

            for(int i = 0; i < PopulationSize; i++)
            {
                ki = tmpAdaptationList[i] / hs;
                adaptationOfIndividualList.Add(Convert.ToInt32(ki));
            }
            //wiedzac ile bedzie kopii kolejnych osobnikow, nalezy wygenerowac ich liste
            int j = 0;
            foreach (Genotype individual in generation)
            {
                try
                {
                    tmpNextGeneration.AddRange(individual.CopyOf(adaptationOfIndividualList[j]));
                }
                catch (IndexOutOfRangeException)
                {
                    Console.Write("Przekroczono " + SigmaPopulationSize + " osobnikow w nowej populacji.");
                    Console.WriteLine(" Zwracam populację i wychodzę z pętli.");
                    return tmpNextGeneration;
                }
                j++;
            }
            //Pozostałe wolne miejsce w nowej populacji zostaną zapełnione osobnikami o największych 
            //częściach ułamkowych oczekiwanych liczb kopii.

            //Console.WriteLine("Proces deterministycznej selekcji osobników potomnych zakończony z sukcesem.");
            return tmpNextGeneration;
        }
        /// <summary>
        /// Oblicza część ułamkową liczby value
        /// </summary>
        /// <param name="value">Liczba zmiennopozycyjna</param>
        /// <returns>Część ułamkowa</returns>
        private decimal Frac(decimal value) { return value - Math.Truncate(value); }

        public void CreateFirstGeneration()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                ThisGeneration.Add(new Genotype(GenotypeSize, this));
            }
        }
    }
}
