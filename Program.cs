using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Rosenbrock
{
    public delegate double FunctionToOptimize(double[] values);

    class Program
    {
        public static System.IO.StreamWriter results = new System.IO.StreamWriter("result_" + DateTime.Now.ToString("HH_mm_ss") + ".txt");

        /// <summary>
        /// Badana funkcja - Rosenbrock
        /// sum(100*(x(i)−x(i−1)^2)^2   + ( 1−x(i−1) )^2 )
        /// </summary>
        public static double Function(double[] values)
        {
            List<double> valueList = new List<double>();
            double value;
            double f1 = 0.0;
            int x = 0;
            int y = 1;
            int counter = 0;

            while(counter < values.Length) 
            {
                value = Math.Pow((1 - values[x]), 2) + 100 * Math.Pow(values[y] - Math.Pow(values[x], 2), 2);
                valueList.Add(value);
                counter++;
            }

            for(int i = 1; i < values.Length; i++) 
            {
                f1 += valueList[i];
            }
            return f1;
        }


        static void Main(string[] args)
        {
            int mu = 70; //liczba osobnikow w populacji P, parametr μ, mu
            int lambda = 230; // liczba osobnikow w populacji O, parametr sigma
            int generationSize = 250; //liczba mozliwych generacji populacji
            int genotypeSize = 2; //rozmiar genotypu, ilość wymiarów funkcji
            float mutationRate = 0.8F; //współczynnik mutacji
            float crossoverRate = 0.8F; //wspołczynnik krzyżowania

            var argsCount = args.Count();
            if (argsCount >= 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("#### Optimization of Rosenbrock function. Enjoy! ####");
                    Console.WriteLine("Please use space between your parameters.");
                    Console.WriteLine("1. Size of population (μ parameter)");
                    Console.WriteLine("2. Size of next population (lambda parameter)");
                    Console.WriteLine("3. Size of generation");
                    Console.WriteLine("4. Size of genotype");
                    Console.WriteLine("5. Mutation rate <0,1>");
                    Console.WriteLine("6. Crossover rate <0,1>");
                    return;
                }
                else if (argsCount > 6 || argsCount < 0)
                {
                    Console.WriteLine("Wrong q-ty of given parameters. Type -help to see help msg.");
                    return;
                }
                else
                {
                    mu = Convert.ToInt32(args[0]);
                    lambda = Convert.ToInt32(args[1]); 
                    generationSize = Convert.ToInt32(args[2]);
                    genotypeSize = Convert.ToInt32(args[3]);
                    mutationRate = Convert.ToSingle(args[4]);
                    crossoverRate = Convert.ToSingle(args[5]);
                }

            }
            Console.WriteLine("Start #Rosenbrock function optymalization. Use standard values....");

            //zmienne tymczasowe
            var function = new FunctionToOptimize(Function);
            var BestGenotypes = new List<double>();

            Console.WriteLine("Otrzymalem dane: ");
            Console.WriteLine("1. Rozmiar pierwszej populacji (mu): " + mu);
            Console.WriteLine("2. Rozmiar populacji potomków (sigma): " + lambda);
            Console.WriteLine("3. Liczba generacji: " + generationSize);
            Console.WriteLine("4. Prawdopodobieństwo mutacji <0,1>: " + mutationRate);
            Console.WriteLine("5. Prawdopodobieństwo krzyżowania: " + crossoverRate);
            Console.WriteLine("6. Rozmiar genotypu (argumentów funkcji): " + genotypeSize);
            var algorithm = new EvolutionStrategy(mu, lambda, generationSize, mutationRate, crossoverRate, genotypeSize, function);
            Program.results.WriteLine("Data: {0} Parametry: mu: {1} lambda: {2}", DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"), mu, lambda);
            //wykonanie algorytmu dokladnie 10 razy
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Iteracja numer: " + i);
                Program.results.WriteLine("Iteracja numer: {0}. Poniżej najlepsze genotypy", i);
                algorithm.Run();
                Program.results.WriteLine("    BEST: {0}", algorithm.BestGenotype.Min().ToString("0.000000000"));
                Console.WriteLine("\t\tBEST: {0}", algorithm.BestGenotype.Min().ToString("0.000000000"));
                BestGenotypes.Add(algorithm.BestGenotype.Min());
            }

            Program.results.WriteLine("KONIEC: Najgorszy: {0}", BestGenotypes.Max().ToString("0.000000000"));
            Program.results.WriteLine("KONIEC: Średni: {0}", BestGenotypes.Average().ToString("0.000000000"));
            Program.results.WriteLine("KONIEC: Najlepszy: {0}", BestGenotypes.Min().ToString("0.000000000"));
            //zamkniecie pliku
            results.WriteLine();
            results.Close();

        }
    }
}
