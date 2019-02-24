using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClebschGordanCoefficients
{
    class Utilities
    {

        public static void DrawGrid(CBScenario scenario)
        {
            var strBldr = new StringBuilder();
            var maxCellStr = "-" + scenario.j1.ToString() + ",-" + scenario.j2.ToString() + "; Y";
            var cellLen = maxCellStr.Length;
            var rowLen = (int) ((cellLen + 3) * (2 * scenario.j + 1));
            Console.WriteLine(strBldr.Clear().Append(char.Parse("-"), rowLen));
            for (Rational m2 = scenario.j2; m2 >= -scenario.j2; m2 -= 1)
            {
                for (int rowPass = 0; rowPass < 2; rowPass++)
                {
                    for (Rational m1 = -scenario.j1; m1 <= scenario.j1; m1 += 1)
                    {
                        var coord = new Tuple<Rational, Rational>(m1, m2);
                        if (scenario.grid.ContainsKey(coord))
                        {
                            if (rowPass == 0)
                            {
                                var str = m1.ToString() + "," + m2.ToString();
                                if (scenario.grid[coord].IsSet)
                                    str += "; Y";
                                Console.Write(str);
                                var diff = maxCellStr.Length - str.Length;
                                Console.Write(strBldr.Clear().Append(char.Parse(" "), diff));
                                Console.Write(" | ");
                            }
                            else
                            {
                                var coefStr = "";
                                if (scenario.grid[coord].IsSet)
                                {
                                    if (scenario.grid[coord].IsNormalized)
                                        coefStr = scenario.grid[coord].normalizedCoefficient.ToString("0.0000") + "(N)";
                                    else
                                        coefStr = scenario.grid[coord].rawCoefficient.ToString("0.0000");
                                }
                                Console.Write(coefStr);
                                var diff = maxCellStr.Length - coefStr.Length;
                                Console.Write(strBldr.Clear().Append(char.Parse(" "), diff));
                                Console.Write(" | ");
                            }

                        }
                        else
                        {
                            Console.Write(strBldr.Clear().Append(char.Parse(" "), cellLen));
                            Console.Write(" | ");
                        }
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.WriteLine(strBldr.Clear().Append(char.Parse("-"), rowLen));
            }
        }

    }
}
