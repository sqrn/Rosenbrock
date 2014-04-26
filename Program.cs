using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            int populationSize = 100;

            EvolutionStrategy strategy = new EvolutionStrategy();
            Genotype genotype = new Genotype(populationSize, strategy);

            genotype.ShowGenes();

            var function = new FunctionToOptimize(Function);

            Console.ReadKey();
        }
    }
}
