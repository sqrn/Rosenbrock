using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rosenbrock
{
    public class GaussianGenerator
    {
        Random _rng = new Random();
        double? _spareValue = null;

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble()
        {
            if (null != _spareValue)
            {
                double tmp = _spareValue.Value;
                _spareValue = null;
                return tmp;
            }

            // Generate two new gaussian values.
            double x, y, sqr;

            // We need a non-zero random point inside the unit circle.
            do
            {
                x = 2.0 * _rng.NextDouble() - 1.0;
                y = 2.0 * _rng.NextDouble() - 1.0;
                sqr = x * x + y * y;
            }
            while (sqr > 1.0 || sqr == 0);

            // Make the Box-Muller transformation.
            double fac = Math.Sqrt(-2.0 * Math.Log(sqr) / sqr);

            _spareValue = x * fac;
            return y * fac;
        }

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble(double mu, double sigma)
        {
            return mu + (NextDouble() * sigma);
        }
    }
}
