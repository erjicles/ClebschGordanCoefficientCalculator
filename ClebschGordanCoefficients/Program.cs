using Radicals;
using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClebschGordanCoefficients
{
    class Program
    {

        // j1
        // j2
        // m1
        // m2
        // j
        // m
        // m1 + m2 = m +- 1
        // abs(m1) <= j1
        // abs(m2) <= j2
        // -j <= m1 + m2 <= j
        // sqrt[(j -+ m)(j +- m + 1)]<m1, m2; j, m +- 1>
        //      = sqrt[(j1 -+ m1 + 1)(j1 +- m1)]<m1 -+ 1, m2; j, m>
        //      + sqrt[(j2 -+ m2 + 1)(j2 +- m2)]<m1, m2 -+ 1; j, m>

        // Fix j1, j2, j

        // J+:
        // sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1>
        //      = sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>
        //      + sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>
        //  ; m1 + m2 = m + 1

        // J-:
        // sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1>
        //      = sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>
        //      + sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>
        //  ; m1 + m2 = m - 1

        // J+:
        // <m1, m2; j, m + 1> = {sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m> + sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j - m)(j + m + 1)]
        // <m1 - 1, m2; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
        // <m1, m2 - 1; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j2 - m2 + 1)(j2 + m2)]

        // J-:
        // <m1, m2; j, m - 1> = {sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m> + sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j + m)(j - m + 1)]
        // <m1 + 1, m2; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
        // <m1, m2 + 1; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j2 + m2 + 1)(j2 - m2)]

        static void Main(string[] args)
        {

            bool continueProgram = true;
            while (continueProgram)
            {
                bool isValidInput = false;

                Rational j1 = 0;
                Rational j2 = 0;
                Rational j = 0;
                Rational m1 = 0;
                Rational m2 = 0;
                Rational m = 0;

                while (!isValidInput)
                {
                    Console.Write("j1: ");
                    j1 = Rational.Parse(Console.ReadLine());
                    Console.Write("j2: ");
                    j2 = Rational.Parse(Console.ReadLine());
                    Console.Write("m: ");
                    m = Rational.Parse(Console.ReadLine());
                    Console.Write("j: ");
                    j = Rational.Parse(Console.ReadLine());
                    Console.Write("m1: ");
                    m1 = Rational.Parse(Console.ReadLine());
                    Console.Write("m2: ");
                    m2 = Rational.Parse(Console.ReadLine());

                    if (Rational.Abs(j1 - j2) > j
                        || j > j1 + j2)
                        Console.WriteLine("Parameters violate |j1 - j2| <= j <= j1 + j2");
                    else if (m1 + m2 != m)
                        Console.WriteLine("Parameters violate m1 + m2 = m");
                    else if (Rational.Abs(m1) > j1)
                        Console.WriteLine("Parameters violate |m1| <= j1");
                    else if (Rational.Abs(m2) > j2)
                        Console.WriteLine("Parameters violate |m2| <= j2");
                    else if (Rational.Abs(m1 + m2) > j)
                        Console.WriteLine("Parameters violate -j <= m1 + m2 <= j");
                    else if (j2.Denominator != m1.Denominator)
                        Console.WriteLine("Parameters violate j1 and m1 must both simultaneously be integers or half integers");
                    else if (j2.Denominator != m2.Denominator)
                        Console.WriteLine("Parameters violate j2 and m2 must both simultaneously be integers or half integers");
                    else
                        isValidInput = true;
                }

                // Run the scenario
                CBScenario scenario = new CBScenario(j1: j1, j2: j2, j: j, m: m);
                scenario.InitializeGrid();
                if (scenario.seedNode == null)
                    Console.Write("-----SEED NODE NOT SET-----");
                scenario.CalculateRawCoefficients();
                scenario.NormalizeCoefficients();
                //Utilities.DrawGrid(scenario);

                var node = scenario.grid[new Tuple<Rational, Rational>(m1, m2)];
                Console.WriteLine("Requested coefficient: " + node.normalizedCoefficient2.ToString());

                Console.WriteLine("Run another scenario? (y/n)");
                var input = Console.ReadLine();
                if ("y".Equals(input, StringComparison.InvariantCultureIgnoreCase)
                    || "yes".Equals(input, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                else
                {
                    continueProgram = false;
                }


            }

        }
    }
}
