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
        public int LambdaPopulationSize { get; set; }
        public int NumberOfGenerations { get; set; }
        public int GenotypeSize { get; set; }

        public Genotype BestGenotype { get; set; }
        public int BestGenotypeGeneration { get; set; }

        public FunctionToOptimize EvaluateFunction;


        public List<Genotype> ThisGeneration;
        public List<Genotype> NextGeneration;
        public List<Genotype> ResultGeneration;

        public EvolutionStrategy(int mu, int lambda, int numberOfGenerations, int genotypeSize, FunctionToOptimize function)
        {
            if (mu % 2 != 0) throw new ArgumentException("Population size must be even!");
            if (lambda % 2 != 0) throw new ArgumentException("Next population size must be even!");

            PopulationSize = mu;
            LambdaPopulationSize = lambda;
            NumberOfGenerations = numberOfGenerations; 
            GenotypeSize = genotypeSize;
            EvaluateFunction = function;

            Random = new Random();
        }

        public void Run()
        {
            ThisGeneration = new List<Genotype>(PopulationSize);//obecna generacja
            NextGeneration = new List<Genotype>(LambdaPopulationSize);//potomkowie
            ResultGeneration = new List<Genotype>(LambdaPopulationSize);//populacja wyjsciowa
            BestGenotype = null;

            CreateFirstGeneration(); //utworzenie pierwszej generacji osobnikow
            RankPopulation();
            //ShowPopulation();
            //Console.WriteLine("Pierwsza generacja. Tworze liste " + ThisGeneration.Count() + " osobników");
            //RankPopulation(ref ThisGeneration);
            for (int i = 0; i < NumberOfGenerations; i++)
            {
                NextGeneration = Reproduction();
                Crossover();
                Mutate();
                //Console.WriteLine("NextGeneration size: " + NextGeneration.Count());
                //O = Krzyzowanie i Mutacja NextGeneration
                //Ocena O
                //ThisGeneration = mu najlepszych osobnikow z O (posortuj tablice i wez mu)
            }

        }

        /// <summary>
        /// Metoda krzyzuje dwa losowo wybrane osobniki, ktore staja sie rodzicami.
        /// I uzupelnia generacje potomkow dokladnie dwoma nowymi osobnikami.
        /// </summary>
        public void Crossover()
        {         
            int one = 0;
            int two = 0;
            for (int i = 0; i < LambdaPopulationSize; i=+2) 
            {
                one = this.Random.Next(LambdaPopulationSize);
                two = this.Random.Next(LambdaPopulationSize);
                ResultGeneration.AddRange(
                    NextGeneration[this.Random.Next(one)].Crossover(NextGeneration[two])
                    );
            }

        }

        public void Mutate()
        {
            foreach(Genotype specimen in ResultGeneration) 
            {
                specimen.Mutate();
            }
        }

        private void RankPopulation(ref List<Genotype> generation)
        {
            foreach (var individual in generation)
            {
                individual.FunctionValue = EvaluateFunction(individual.GetValues());
            }
            //sortowanie
            //generation.Sort(new GenotypeComparer());
        }
        public double RankPopulation()
        {
            double hs; //przystosowanie populacji
            double hi; //srednie przystosowanie osobnika
            var tmpAdaptationList = new List<double>();
            foreach (Genotype individual in ThisGeneration)
            {
                hi = individual.GetValues().Sum();
                tmpAdaptationList.Add(hi);
            }
            hs = tmpAdaptationList.Sum() / PopulationSize;
            Console.WriteLine("Ocena populacji: " + hs);
            return hs;
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
            var tmpNextGeneration = new List<Genotype>(LambdaPopulationSize);//potomkowie
            var individualMantissaList = new List<double>();//tablica czesci ulamkowej
            double ki; //oczekiwana liczba kopii osobnika
            double hs = RankPopulation(); //srednie przystosowanie calej populacji

            int howCopy = 0;
            for(int i = 0; i < PopulationSize; i++)
            {
                ki = generation[i].RankIndividual() / hs; //oblicz przystosowanie osobnika
                // od razu dopisz do nowej generacji
                howCopy = (int)Math.Round(ki,0); //ile razy (wartosc calkowita z ki)
                //Console.WriteLine("Bede kopiowac dokladnie: " + howCopy);
                for(int j = 0; j < howCopy; j++)
                {
                    //moze sie zdazyc sytuacja gdy tmpNextGeneration bedzie przepelniony
                    try 
                    {
                        tmpNextGeneration.Add(generation[i]);
                    } catch (ArgumentOutOfRangeException) 
                    {
                        return tmpNextGeneration; //poniewaz wtedy bedzie cala tablica pelna
                    }
                }
            } // koniec uzupelniania po przystosowaniu osobnikow
            // jezeli lista nowej populacji tymczasowej nie jest pelna (do rozmiaru sigma)
            // wtedy dopelnij liste osobnikami z najwieksza czescia ulamkowa
            double maxkiu = 0.0;
            int k = 0;
            int index = 0;
            var indexesChecked = new List<int>();
            for (int i = 0; i < PopulationSize; i++)
            {
                //sprawdz czy tablica osobnikow tymczasowych jest juz zapelniona
                if (tmpNextGeneration.Count() == LambdaPopulationSize)
                    return tmpNextGeneration;
                
                //wez czesc ulamkowa pierwszego osobnika
                //i przeszukaj cala populacje i znajdz osobnika z najwieksza czescia ulamkowa
                maxkiu =  Frac(generation[i].RankIndividual() / hs);
                for( ; k < PopulationSize; k++) 
                {
                    if(indexesChecked.Contains(k))
                        continue;
                    if(maxkiu < Frac(generation[k].RankIndividual() / hs)) 
                    {
                        maxkiu = Frac(generation[k].RankIndividual() / hs);
                        index = generation.IndexOf(generation[k]);
                    }
                }
                //dodaj najlepszego osobnika do tablicy
                tmpNextGeneration.Add(generation[index]);
                //oznacz osobnik jako odwiedzony aby zapobiec kopiowaniu tego samego osobnika wielokrotnie
                indexesChecked.Add(index);
                index = 0;//wyzeruj index

            }

            Console.WriteLine(
                "Proces deterministycznej selekcji osobników potomnych zakończony przed zapelnieniem calej listy."
                );
            return tmpNextGeneration;
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
