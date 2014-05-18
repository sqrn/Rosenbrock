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
        /// <summary>
        /// Badana funkcja - Rosenbrock
        /// sum(100*(x(i)−x(i−1)^2)^2   + ( 1−x(i−1) )^2 )
        /// </summary>
        public static double Function(double[] values)
        {
            List<double> valueList = new List<double>();
            double value;

            for (int i = 1; i < values.Length - 1; i++)
            {
                value = 100 * Math.Pow(values[i] - Math.Pow(values[i - 1], 2), 2) + Math.Pow(1 - values[i - 1], 2);
                valueList.Add(value);
            }
            double f1 = valueList.Sum();
            //Console.WriteLine("Wynik funkcji to: " + f1);
            return f1;
        }


        static void Main(string[] args)
        {
            //double[] a = {1.2, 2.1, 3.5};
            //Function(a);

            int populationSize = 50; //liczba osobnikow w populacji P, parametr μ, mu
            int sigmaPopulationSize = 300; // liczba osobnikow w populacji O, parametr sigma
            int generationSize = 1000; //liczba mozliwych generacji populacji
            int genotypeSize = 2; //rozmiar genotypu, ilość wymiarów funkcji

            var argsCount = args.Count();
            if (argsCount >= 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("#### Optimization of Rosenbrock function. Enjoy! ####");
                    Console.WriteLine("Please use space between your parameters.");
                    Console.WriteLine("1. Size of population (μ parameter)");
                    Console.WriteLine("2. Size of next population (sigma parameter)");
                    Console.WriteLine("3. Size of generation");
                    Console.WriteLine("4. Size of genotype");
                    return;
                }
                else if (argsCount != 5)
                {
                    Console.WriteLine("Wrong q-ty of given parameters. Type -help to see help msg.");
                    return;
                }
                else
                {
                    populationSize = Convert.ToInt32(args[0]);
                    sigmaPopulationSize = Convert.ToInt32(args[1]); 
                    generationSize = Convert.ToInt32(args[2]);
                    genotypeSize = Convert.ToInt32(args[3]);
                }

            }
            Console.WriteLine("Start #Rosenbrock function optymalization. Use standard values....");

            //zmienne tymczasowe
            var function = new FunctionToOptimize(Function);
            var bestGenotypes = new List<Genotype>();
            var bestGenotypesGeneration = new List<int>();

            //wykonanie algorytmu dokladnie 10 razy
            for (int i = 1; i <= 10; i++)
            {
                var algorithm = new EvolutionStrategy(populationSize, sigmaPopulationSize, generationSize, genotypeSize, function);
                Console.WriteLine("Rozpoczynam proces tworzenia " + i + " cywilizacji osobników.");
                algorithm.Run();
                //GaussianGenerator gaussian = new GaussianGenerator();

                ///Console.ReadKey();

                //zapisz najlepszych osobnikow
                //bestGenotypes.Add(algorithm.BestGenotype);
                //bestGenotypesGeneration.Add(algorithm.BestGenotypeGeneration);
            }
            Console.WriteLine("Koniec... Naciśnij ENTER aby zakończyć.");


            /*
            //zapis do pliku
            FileStream fs = new FileStream(DateTime.Now.ToString("HH-mm-ss") + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);

            Console.WriteLine("Generacja najlepszego Genotypu; Wartość najlepszego Genotypu;");
           
            for (int i = 0; i < 10; i++)
			{
                
                Console.WriteLine("{0};{1}", bestGenotypesGeneration[i], bestGenotypes[i].FunctionValue.ToString("0.000000000"));
			}

            Console.WriteLine("Średnia wartość;Nalepsza wartość;Najgorsza wartość");
            Console.WriteLine("{0};{1};{2}", bestGenotypes.Average(g => g.FunctionValue), bestGenotypes.Min(g => g.FunctionValue), bestGenotypes.Max(g => g.FunctionValue));
            Console.WriteLine("Średnia ilość epok;Nalepsza ilość epok;Najgorsza ilość epok");
            Console.WriteLine("{0};{1};{2}", bestGenotypesGeneration.Average(), bestGenotypesGeneration.Min(), bestGenotypesGeneration.Max());
            sw.Flush();
            fs.Flush(true);
            fs.Close();
            */
            Console.ReadKey();
        }
    }
}
