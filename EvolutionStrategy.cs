using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Rosenbrock
{
    class EvolutionStrategy
    {
        public Random Random;
        private int PopulationSize { get; set; }
        private int LambdaPopulationSize { get; set; }
        private int NumberOfGenerations { get; set; }
        private float MutationRate { get; set; }
        private float CrossoverRate { get; set; }
        private int GenotypeSize { get; set; }

        public List<double> BestGenotype { get; set; }
        public int BestGenotypeGeneration { get; set; }

        public FunctionToOptimize EvaluateFunction;


        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;
        public List<Genotype> ResultGeneration;

        public EvolutionStrategy (
            int mu, 
            int lambda, 
            int numberOfGenerations,
            float mutationRate,
            float crossoverRate,
            int genotypeSize, 
            FunctionToOptimize function )
        {
            if (mu % 2 != 0) throw new ArgumentException("Population size must be even!");
            if (lambda % 2 != 0) throw new ArgumentException("Next population size must be even!");
            if (mutationRate >= 1.0 || mutationRate <= 0) throw new ArgumentOutOfRangeException("mutationRate must be between 0 and 1");
            if (crossoverRate >= 1.0 || crossoverRate <= 0) throw new ArgumentOutOfRangeException("crossoverRate must be between 0 and 1");

            PopulationSize = mu;
            LambdaPopulationSize = lambda;
            NumberOfGenerations = numberOfGenerations;
            MutationRate = mutationRate;
            CrossoverRate = crossoverRate;
            GenotypeSize = genotypeSize;
            EvaluateFunction = function;

            Random = new Random();
        }

        public void Run()
        {
            ThisGeneration = new List<Genotype>(PopulationSize);//obecna generacja
            NextGeneration = new List<Genotype>(LambdaPopulationSize);//potomkowie
            ResultGeneration = new List<Genotype>(LambdaPopulationSize);//populacja wyjsciowa

            CreateFirstGeneration(); //utworzenie pierwszej generacji osobnikow
            RatePopulation(ref ThisGeneration);
            BestGenotype = new List<double>(PopulationSize);
            //ShowPopulation(ThisGeneration);
            for (int i = 0; i <= NumberOfGenerations; i++)
            {
                Console.Write("{0} ", i);
                Program.results.WriteLine("{0}", ThisGeneration.Last().FunctionValue.ToString("0.000000000"));
                BestGenotype.Add(ThisGeneration.Last().FunctionValue);
                NextGeneration = Selection();
                Crossover();
                Mutate();
                //po mutacji i krzyzowaniu otrzymujemy nowa populacje - ResultGeneration
                RatePopulation(ref ResultGeneration);
                ThisGeneration = Succesion();
            }

        }

        private void RatePopulation(ref List<Genotype> generation)
        {
            foreach (var specimen in generation)
            {
                specimen.FunctionValue = EvaluateFunction( specimen.GetValues() );
            }
            //sortowanie
            generation.Sort(new GenotypeComparer() );
        }

        /// <summary>
        /// Metoda krzyzuje dwa losowo wybrane osobniki, ktore staja sie rodzicami.
        /// I uzupelnia generacje potomkow dokladnie dwoma nowymi osobnikami.
        /// </summary>
        public void Crossover()
        {
            var lambda = LambdaPopulationSize;
            for (int i = 0; i < lambda; i+=2)
            {
                if(this.Random.NextDouble() < CrossoverRate)
                    ResultGeneration.AddRange(
                        Genotype.Crossover(
                            NextGeneration[Random.Next(lambda)],
                            NextGeneration[Random.Next(lambda)]
                            )
                        );
            }
        }

        public void Mutate()
        {
            foreach(Genotype specimen in ResultGeneration) 
            {
                specimen.Mutate(MutationRate);
            }
        }

        public void ShowPopulation(List<Genotype> generation)
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                //Console.WriteLine(ThisGeneration[i].GetValues()[0] + "; " + ThisGeneration[i].GetValues()[1]);
                Console.WriteLine(ThisGeneration[i].FunctionValue.ToString("0.000000000"));
            }
        }

        /// <summary>
        /// Ocena populacji na podstawie wartości funkcji
        /// </summary>
        /// <returns>średnia ocena całej populacji</returns>
        public double RankPopulation()
        {
            double sum = 0.0; //srednie przystosowanie osobnika
            foreach (Genotype specimen in ThisGeneration)
            {
                sum += specimen.FunctionValue;
            }
            return sum / PopulationSize;
        }

        /// <summary>
        /// Selekcja deterministyczna. Funkcja ocenia przystosowanie każdego osobnika indywidualnie
        /// Następnie kopiuje go do populacji tymczasowej dokładnie tyle razy ile wynosi część całkowita oceny przystosowania.
        /// Ocena przystosowania osobnika / Średnie przystosowanie wszystkich osobników w populacji
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Genotype> Selection() 
        {
            NextGeneration.Clear();

            var tmpGenotypes = new List<Genotype>(LambdaPopulationSize);
            double sum = RankPopulation();//srednie przystosowanie populacji
            bool isLambdaEmpty = true;
            int qty;
            while (isLambdaEmpty)
            {
                foreach (Genotype specimen in ThisGeneration)
                {
                    qty = (int)(Math.Round(specimen.FunctionValue / sum, 0));
                    for (int j = 0; j < qty; j++)
                    {
                        tmpGenotypes.Add(specimen);
                        if (tmpGenotypes.Count == LambdaPopulationSize)
                            return tmpGenotypes;
                    }
                }
                if (tmpGenotypes.Count == LambdaPopulationSize)
                {
                    isLambdaEmpty = false;
                    continue;
                }
                //czesci ulamkowe jezeli dojdzie do kontynuacji petli
                foreach (Genotype specimen in ThisGeneration)
                {
                    qty = (int)(Math.Round(Frac(specimen.FunctionValue / sum), 0));
                    for (int j = 0; j < qty; j++)
                    {
                        tmpGenotypes.Add(specimen);
                        if (tmpGenotypes.Count == LambdaPopulationSize)
                            return tmpGenotypes;
                    }
                }
                if (tmpGenotypes.Count == LambdaPopulationSize)
                    isLambdaEmpty = false;
            }

            return tmpGenotypes;
        }

        public List<Genotype> Succesion()
        {
            ThisGeneration.Clear();
            var tmpGeneration = new List<Genotype>(PopulationSize);
            tmpGeneration.AddRange(ResultGeneration.Take(PopulationSize));
            tmpGeneration.Sort(new GenotypeComparer());
            return tmpGeneration;
        }

        /// <summary>
        /// Oblicza część ułamkową liczby value
        /// </summary>
        /// <param name="value">Liczba zmiennopozycyjna</param>
        /// <returns>Część ułamkowa</returns>
        public static double Frac(double value) { return value - Math.Truncate(value); }

        public void CreateFirstGeneration()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                ThisGeneration.Add(new Genotype(GenotypeSize, this));
            }
        }

    }
}
