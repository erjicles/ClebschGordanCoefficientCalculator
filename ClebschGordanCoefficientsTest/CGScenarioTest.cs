using ClebschGordanCoefficients;
using Radicals;
using Rationals;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace ClebschGordanCoefficientsTest
{
    public class CGScenarioTest
    {
        [Fact]
        public void CGCalculatorTest()
        {
            // Test cases taken from here:
            // https://en.wikipedia.org/wiki/Table_of_Clebsch%E2%80%93Gordan_coefficients
            // The cases are formatted as:
            // Items 1-6: j1, j2, m, j, m1, m2 
            // Item 7: Expected result (in radical form)
            var testCases = new List<Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>>()
            {
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, 1, 1, (Rational)1/2, (Rational)1/2,
                    Radical.One),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, -1, 1, -(Rational)1/2, (Rational)(-1)/2,
                    Radical.One),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, 0, 1, (Rational)1/2, (Rational)(-1)/2,
                    Radical.Sqrt((Rational)1/2)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, 0, 1, (Rational)(-1)/2, (Rational)1/2,
                    Radical.Sqrt((Rational)1/2)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, 0, 0, (Rational)1/2, (Rational)(-1)/2,
                    Radical.Sqrt((Rational)1/2)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)1/2, (Rational)1/2, 0, 0, (Rational)(-1)/2, (Rational)1/2,
                    -Radical.Sqrt((Rational)1/2)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)5/2, 2, (Rational)1/2, (Rational)7/2, (Rational)3/2, -1,
                    Radical.Sqrt((Rational)121/315)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)5/2, 2, (Rational)1/2, (Rational)3/2, (Rational)(-3)/2, 2,
                    -Radical.Sqrt((Rational)32/105)),
                new Tuple<Rational, Rational, Rational, Rational, Rational, Rational, Radical>(
                    (Rational)5/2, (Rational)5/2, 0, 4, (Rational)(-3)/2, (Rational)3/2,
                    -Radical.Sqrt((Rational)9/28)),
            };
            foreach (var testCase in testCases)
            {
                var j1 = testCase.Item1;
                var j2 = testCase.Item2;
                var m = testCase.Item3;
                var j = testCase.Item4;
                var m1 = testCase.Item5;
                var m2 = testCase.Item6;
                var expected = testCase.Item7;
                CBScenario scenario = new CBScenario(j1: j1, j2: j2, j: j, m: m);
                scenario.InitializeGrid();
                scenario.CalculateRawCoefficients();
                scenario.NormalizeCoefficients();
                var actual = scenario.grid[new Tuple<Rational, Rational>(m1, m2)];
                var isEqual = expected == actual.normalizedCoefficient;
                Assert.True(isEqual);
            }
        }
    }
}
