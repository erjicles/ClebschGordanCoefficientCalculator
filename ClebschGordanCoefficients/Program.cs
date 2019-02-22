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

        public class CBNode
        {
            public int m1 { get; set; } = 0;
            //public int denominatorM1 = 0;
            public int m2 { get; set; } = 0;
            //public int denominatorM2 = 0;
            public Tuple<int, int> GridCoordinate { get; set; }
            public bool IsSet { get; set; } = false;
            public bool IsSeedNode { get; set; } = false;
            public double rawCoefficient { get; set; } = 0.0;
            public double normalizedCoefficient { get; set; } = 0.0;
            public bool IsNormalized { get; set; } = false;
            public CBNode Neighbor_M1minus1_M2 { get; set; }
            public CBNode Neighbor_M1_M2minus1 { get; set; }
            public CBNode Neighbor_M1_M2plus1 { get; set; }
            public CBNode Neighbor_M1plus1_M2 { get; set; }
            public CBNode Neighbor_M1plus1_M2minus1 { get; set; }
            public CBNode Neighbor_M1plus1_M2plus1 { get; set; }
            public CBNode Neighbor_M1minus1_M2minus1 { get; set; }
            public CBNode Neighbor_M1minus1_M2plus1 { get; set; }

            public CBNode(int m1, int m2)
            {
                this.m1 = m1;
                this.m2 = m2;
                this.GridCoordinate = new Tuple<int, int>(m1, m2);
            }

            public bool CalculateRawCoefficient(int j1, int j2, int j)
            {
                // Check if we can calculate the raw coefficient
                if (IsSet)
                    return false;

                //  x   O
                //      -
                int calculationMethod = 0;
                if (Neighbor_M1minus1_M2 != null
                    && Neighbor_M1minus1_M2.IsSet
                    && Neighbor_M1_M2minus1 == null)
                    calculationMethod = 1;

                //  -   O
                //      x
                if (Neighbor_M1minus1_M2 == null
                    && Neighbor_M1_M2minus1 != null
                    && Neighbor_M1_M2minus1.IsSet)
                    calculationMethod = 2;

                //  x   O
                //      x
                if (Neighbor_M1minus1_M2 != null
                    && Neighbor_M1minus1_M2.IsSet
                    && Neighbor_M1_M2minus1 != null
                    && Neighbor_M1_M2minus1.IsSet)
                    calculationMethod = 3;

                //  -
                //  O   x
                if (Neighbor_M1plus1_M2 != null
                    && Neighbor_M1plus1_M2.IsSet
                    && Neighbor_M1_M2plus1 == null)
                    calculationMethod = 4;

                //  x
                //  O   -
                if (Neighbor_M1plus1_M2 == null
                    && Neighbor_M1_M2plus1 != null
                    && Neighbor_M1_M2plus1.IsSet)
                    calculationMethod = 5;

                //  x
                //  O   x
                if (Neighbor_M1plus1_M2 != null
                    && Neighbor_M1plus1_M2.IsSet
                    && Neighbor_M1_M2plus1 != null
                    && Neighbor_M1_M2plus1.IsSet)
                    calculationMethod = 6;

                //  O
                //  x   -
                if (Neighbor_M1_M2minus1 != null
                    && Neighbor_M1_M2minus1.IsSet
                    && Neighbor_M1plus1_M2minus1 == null)
                    calculationMethod = 7;

                //  O
                //  -   x
                if (Neighbor_M1_M2minus1 == null
                    && Neighbor_M1plus1_M2minus1 != null
                    && Neighbor_M1plus1_M2minus1.IsSet)
                    calculationMethod = 8;

                //  O
                //  x   x
                if (Neighbor_M1_M2minus1 != null
                    && Neighbor_M1_M2minus1.IsSet
                    && Neighbor_M1plus1_M2minus1 != null
                    && Neighbor_M1plus1_M2minus1.IsSet)
                    calculationMethod = 9;

                //  -
                //  x   O
                if (Neighbor_M1minus1_M2 != null
                    && Neighbor_M1minus1_M2.IsSet
                    && Neighbor_M1minus1_M2plus1 == null)
                    calculationMethod = 10;

                //  x
                //  -   O
                if (Neighbor_M1minus1_M2 == null
                    && Neighbor_M1minus1_M2plus1 != null
                    && Neighbor_M1minus1_M2plus1.IsSet)
                    calculationMethod = 11;

                //  x
                //  x   O
                if (Neighbor_M1minus1_M2 != null
                    && Neighbor_M1minus1_M2.IsSet
                    && Neighbor_M1minus1_M2plus1 != null
                    && Neighbor_M1minus1_M2plus1.IsSet)
                    calculationMethod = 12;

                //  O   x
                //      -
                if (Neighbor_M1plus1_M2 != null
                    && Neighbor_M1plus1_M2.IsSet
                    && Neighbor_M1plus1_M2minus1 == null)
                    calculationMethod = 13;

                //  O   -
                //      x
                if (Neighbor_M1plus1_M2 == null
                    && Neighbor_M1plus1_M2minus1 != null
                    && Neighbor_M1plus1_M2minus1.IsSet)
                    calculationMethod = 14;

                //  O   x
                //      x
                if (Neighbor_M1plus1_M2 != null
                    && Neighbor_M1plus1_M2.IsSet
                    && Neighbor_M1plus1_M2minus1 != null
                    && Neighbor_M1plus1_M2minus1.IsSet)
                    calculationMethod = 15;

                //  -   x
                //      O
                if (Neighbor_M1minus1_M2plus1 == null
                    && Neighbor_M1_M2plus1 != null
                    && Neighbor_M1_M2plus1.IsSet)
                    calculationMethod = 16;

                //  x   -
                //      O
                if (Neighbor_M1minus1_M2plus1 != null
                    && Neighbor_M1minus1_M2plus1.IsSet
                    && Neighbor_M1_M2plus1 == null)
                    calculationMethod = 17;

                //  x   x
                //      O
                if (Neighbor_M1minus1_M2plus1 != null
                    && Neighbor_M1minus1_M2plus1.IsSet
                    && Neighbor_M1_M2plus1 != null
                    && Neighbor_M1_M2plus1.IsSet)
                    calculationMethod = 18;

                if (calculationMethod == 0)
                    return false;

                var M1 = m1 / 2.0;
                var M2 = m2 / 2.0;
                var M = M1 + M2;
                var J1 = j1 / 2.0;
                var J2 = j2 / 2.0;
                var J = j / 2.0;
                if (calculationMethod == 1)
                {
                    //  x   O
                    //      -
                    // J+:
                    // <m1, m2; j, m + 1> = {sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j - m)(j + m + 1)]
                    M -= 1;
                    rawCoefficient =
                        (Math.Sqrt((J1 - M1 + 1)*(J1 + M1)) 
                            * Neighbor_M1minus1_M2.rawCoefficient)
                        / Math.Sqrt((J - M) * (J + M + 1));
                }
                else if (calculationMethod == 2)
                {
                    //  -   O
                    //      x
                    // J+:
                    // <m1, m2; j, m + 1> = {sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j - m)(j + m + 1)]
                    M -= 1;
                    rawCoefficient =
                        (Math.Sqrt((J2 - M2 + 1) * (J2 + M2))
                            * Neighbor_M1_M2minus1.rawCoefficient)
                        / Math.Sqrt((J - M) * (J + M + 1));
                }
                else if (calculationMethod == 3)
                {
                    //  x   O
                    //      x
                    // J+:
                    // <m1, m2; j, m + 1> = {sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m> + sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j - m)(j + m + 1)]
                    M -= 1;
                    rawCoefficient =
                        (
                            (Math.Sqrt((J2 - M1 + 1) * (J1 + M1))
                                * Neighbor_M1minus1_M2.rawCoefficient)
                            + (Math.Sqrt((J2 - M2 + 1) * (J2 + M2))
                                * Neighbor_M1_M2minus1.rawCoefficient)
                                )
                        / Math.Sqrt((J - M) * (J + M + 1));
                }
                else if (calculationMethod == 4)
                {
                    //  -
                    //  O   x
                    // J-:
                    // <m1, m2; j, m - 1> = {sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j + m)(j - m + 1)]
                    M += 1;
                    rawCoefficient =
                        (Math.Sqrt((J1 + M1 + 1) * (J1 - M1))
                            * Neighbor_M1plus1_M2.rawCoefficient)
                        / Math.Sqrt((J + M) * (J - M + 1));
                }
                else if (calculationMethod == 5)
                {
                    //  x
                    //  O   -
                    // J-:
                    // <m1, m2; j, m - 1> = {sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j + m)(j - m + 1)]
                    M += 1;
                    rawCoefficient =
                        (Math.Sqrt((J2 + M2 + 1) * (J2 - M2))
                            * Neighbor_M1_M2plus1.rawCoefficient)
                        / Math.Sqrt((J + M) * (J - M + 1));
                }
                else if (calculationMethod == 6)
                {
                    //  x
                    //  O   x
                    // J-:
                    // <m1, m2; j, m - 1> = {sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m> + sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j + m)(j - m + 1)]
                    M += 1;
                    rawCoefficient =
                        (
                            (Math.Sqrt((J1 + M1 + 1) * (J1 - M1))
                                * Neighbor_M1plus1_M2.rawCoefficient)
                            + (Math.Sqrt((J2 + M2 + 1) * (J2 - M2))
                                * Neighbor_M1_M2plus1.rawCoefficient))
                        / Math.Sqrt((J + M) * (J - M + 1));
                }
                else if (calculationMethod == 7)
                {
                    //  O
                    //  x   -
                    // J-:
                    // <m1, m2 + 1; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1>} / sqrt[(j2 + m2 + 1)(j2 - m2)]
                    M2 = M2 - 1;
                    M = M1 + M2 + 1;
                    rawCoefficient =
                        (Math.Sqrt((J + M) * (J - M + 1))
                            * Neighbor_M1_M2minus1.rawCoefficient)
                        / Math.Sqrt((J2 + M2 + 1) * (J2 - M2));
                }
                else if (calculationMethod == 8)
                {
                    //  O
                    //  -   x
                    // J-:
                    // <m1, m2 + 1; j, m> = {-sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j2 + m2 + 1)(j2 - m2)]
                    M2 = M2 - 1;
                    M = M1 + m2 + 1;
                    rawCoefficient =
                        (-Math.Sqrt((J1 + m1 + 1) * (J1 - M1))
                            * Neighbor_M1plus1_M2minus1.rawCoefficient)
                        / Math.Sqrt((J2 + M2 + 1) * (J2 - M2));
                }
                else if (calculationMethod == 9)
                {
                    //  O
                    //  x   x
                    // J-:
                    // <m1, m2 + 1; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j2 + m2 + 1)(j2 - m2)]
                    M2 = M2 - 1;
                    M = M1 + M2 + 1;
                    rawCoefficient = 
                        (
                            (Math.Sqrt((J + M) * (J - M + 1))
                                * Neighbor_M1_M2minus1.rawCoefficient)
                            - (Math.Sqrt((J1 + m1 + 1) * (J1 - M1))
                                * Neighbor_M1plus1_M2minus1.rawCoefficient))
                        / Math.Sqrt((J2 + M2 + 1) * (J2 - M2));
                }
                else if (calculationMethod == 10)
                {
                    //  -
                    //  x   O
                    // J-:
                    // <m1 + 1, m2; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
                    M1 = M1 - 1;
                    M = M1 + M2 + 1;
                    rawCoefficient =
                        (Math.Sqrt((J + M) * (J - M + 1))
                            * Neighbor_M1minus1_M2.rawCoefficient)
                        / Math.Sqrt((J1 + M1 + 1) * (J1 - M1));
                }
                else if (calculationMethod == 11)
                {
                    //  x
                    //  -   O
                    // J-:
                    // <m1 + 1, m2; j, m> = {-sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
                    M1 = M1 - 1;
                    M = M1 + M2 + 1;
                    rawCoefficient =
                        (-Math.Sqrt((J2 + M2 + 1) * (J2 - M2))
                            * Neighbor_M1minus1_M2plus1.rawCoefficient)
                        / Math.Sqrt((J1 + M1 + 1) * (J1 - M1));
                }
                else if (calculationMethod == 12)
                {
                    //  x
                    //  x   O
                    // J-:
                    // <m1 + 1, m2; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
                    M1 = M1 - 1;
                    M = M1 + M2 + 1;
                    rawCoefficient =
                        (
                            (Math.Sqrt((J + M) * (J - M + 1))
                                * Neighbor_M1minus1_M2.rawCoefficient)
                            - (Math.Sqrt((J2 + M2 + 1) * (J2 - M2))
                                * Neighbor_M1minus1_M2plus1.rawCoefficient))
                        / Math.Sqrt((J1 + M1 + 1) * (J1 - M1));
                }
                else if (calculationMethod == 13)
                {
                    //  O   x
                    //      -
                    // J+:
                    // <m1 - 1, m2; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
                    M1 = M1 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (Math.Sqrt((J - M) * (J + M + 1))
                            * Neighbor_M1plus1_M2.rawCoefficient)
                        / Math.Sqrt((J1 - M1 + 1) * (J1 + M1));
                }
                else if (calculationMethod == 14)
                {
                    //  O   -
                    //      x
                    // J+:
                    // <m1 - 1, m2; j, m> = {-sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
                    M1 = M1 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (-Math.Sqrt((J2 - M2 + 1) * (J2 + M2))
                            * Neighbor_M1plus1_M2minus1.rawCoefficient)
                        / Math.Sqrt((J1 - M1 + 1) * (J1 + M1));
                }
                else if (calculationMethod == 15)
                {
                    //  O   x
                    //      x
                    // J+:
                    // <m1 - 1, m2; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
                    M1 = M1 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (
                            (Math.Sqrt((J - M) * (J + M + 1))
                                * Neighbor_M1plus1_M2.rawCoefficient)
                            - (Math.Sqrt((J2 - M2 + 1) * (J2 + M2))
                                * Neighbor_M1plus1_M2minus1.rawCoefficient))
                        / Math.Sqrt((J1 - M1 + 1) * (J1 + M1));
                }
                else if (calculationMethod == 16)
                {
                    //  -   x
                    //      O
                    // J+:
                    // <m1, m2 - 1; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1>} / sqrt[(j2 - m2 + 1)(j2 + m2)]
                    M2 = M2 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (Math.Sqrt((J - M) * (J + M + 1))
                            * Neighbor_M1_M2plus1.rawCoefficient)
                        / Math.Sqrt((J2 - M2 + 1) * (J2 + M2));
                }
                else if (calculationMethod == 17)
                {
                    //  x   -
                    //      O
                    // J+:
                    // <m1, m2 - 1; j, m> = {-sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j2 - m2 + 1)(j2 + m2)]
                    M2 = M2 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (-Math.Sqrt((J1 - M1 + 1) * (J1 + M1))
                            * Neighbor_M1minus1_M2plus1.rawCoefficient)
                        / Math.Sqrt((J2 - M2 + 1) * (J2 + M2));
                }
                else if (calculationMethod == 18)
                {
                    //  x   x
                    //      O
                    // J+:
                    // <m1, m2 - 1; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j2 - m2 + 1)(j2 + m2)]
                    M2 = M2 + 1;
                    M = M1 + M2 - 1;
                    rawCoefficient =
                        (
                            (Math.Sqrt((J - M) * (J + M + 1))
                                * Neighbor_M1_M2plus1.rawCoefficient)
                            - (Math.Sqrt((J1 - M1 + 1) * (J1 + M1))
                                * Neighbor_M1minus1_M2plus1.rawCoefficient))
                        / Math.Sqrt((J2 - M2 + 1) * (J2 + M2));
                }
                else
                {
                    throw new Exception("Invalid calculation method: " + calculationMethod.ToString());
                }

                IsSet = true;


                return true;
            }
        }

        public class CBScenario
        {
            public int j1 { get; set; }
            public int j2 { get; set; }
            public int j { get; set; }
            public int m { get; set; }
            public Dictionary<Tuple<int, int>, CBNode> grid { get; set; }
            public List<CBNode> unprocessedNodeList { get; set; }
            public CBNode seedNode { get; set; }


            public CBScenario(int j1, int j2, int j, int m)
            {
                this.j1 = j1;
                this.j2 = j2;
                this.j = j;
                this.m = m;
            }

            public void InitializeGrid()
            {
                grid = new Dictionary<Tuple<int, int>, CBNode>();
                unprocessedNodeList = new List<CBNode>();
                // Create nodes
                for (int m1 = -j1; m1 <= j1; m1+=2)
                {
                    for (int m2 = -j2; m2 <= j2; m2+=2)
                    {
                        if (Math.Abs(m1 + m2) <= j
                            && Math.Abs(m1) <= j1
                            && Math.Abs(m2) <= j2)
                        {
                            var node = new CBNode(m1, m2);
                            if (grid.ContainsKey(node.GridCoordinate))
                                throw new Exception("Node already created: m1: " + m1.ToString() + "; m2: " + m2.ToString());
                            grid.Add(node.GridCoordinate, node);
                            unprocessedNodeList.Add(node);
                        }
                    }
                }
                // Populate neighbors
                for (int m1 = -j1; m1 <= j1; m1+=2)
                {
                    for (int m2 = -j2; m2 <= j2; m2+=2)
                    {
                        var coord = new Tuple<int, int>(m1, m2);
                        if (grid.ContainsKey(coord))
                        {
                            var node = grid[coord];

                            var coord_M1minus1_M2 = new Tuple<int, int>(m1 - 2, m2);
                            var coord_M1_M2minus1 = new Tuple<int, int>(m1, m2 - 2);
                            var coord_M1_M2plus1 = new Tuple<int, int>(m1, m2 + 2);
                            var coord_M1plus1_M2 = new Tuple<int, int>(m1 + 2, m2);
                            var coord_M1plus1_M2minus1 = new Tuple<int, int>(m1 + 2, m2 - 2);
                            var coord_M1plus1_M2plus1 = new Tuple<int, int>(m1 + 2, m2 + 2);
                            var coord_M1minus1_M2minus1 = new Tuple<int, int>(m1 - 2, m2 - 2);
                            var coord_M1minus1_M2plus1 = new Tuple<int, int>(m1 - 2, m2 + 2);
                            
                            if (grid.ContainsKey(coord_M1minus1_M2))
                                node.Neighbor_M1minus1_M2 = grid[coord_M1minus1_M2];
                            if (grid.ContainsKey(coord_M1_M2minus1))
                                node.Neighbor_M1_M2minus1 = grid[coord_M1_M2minus1];
                            if (grid.ContainsKey(coord_M1_M2plus1))
                                node.Neighbor_M1_M2plus1 = grid[coord_M1_M2plus1];
                            if (grid.ContainsKey(coord_M1plus1_M2))
                                node.Neighbor_M1plus1_M2 = grid[coord_M1plus1_M2];
                            if (grid.ContainsKey(coord_M1plus1_M2minus1))
                                node.Neighbor_M1plus1_M2minus1 = grid[coord_M1plus1_M2minus1];
                            if (grid.ContainsKey(coord_M1plus1_M2plus1))
                                node.Neighbor_M1plus1_M2plus1 = grid[coord_M1plus1_M2plus1];
                            if (grid.ContainsKey(coord_M1minus1_M2minus1))
                                node.Neighbor_M1minus1_M2minus1 = grid[coord_M1minus1_M2minus1];
                            if (grid.ContainsKey(coord_M1minus1_M2plus1))
                                node.Neighbor_M1minus1_M2plus1 = grid[coord_M1minus1_M2plus1];
                        }
                    }
                }
                // Set the seed node
                for (int m1 = -j1; m1 <= j1; m1+=2)
                {
                    for (int m2 = -j2; m2 <= j2; m2+=2)
                    {
                        var coord = new Tuple<int, int>(m1, m2);
                        if (grid.ContainsKey(coord))
                        {
                            var node = grid[coord];
                            if ((node.Neighbor_M1minus1_M2 != null
                                    && node.Neighbor_M1_M2minus1 == null)
                                || (node.Neighbor_M1minus1_M2 == null
                                    && node.Neighbor_M1_M2minus1 != null)
                                || (node.Neighbor_M1plus1_M2 != null
                                    && node.Neighbor_M1_M2plus1 == null)
                                || (node.Neighbor_M1plus1_M2 == null
                                    && node.Neighbor_M1_M2plus1 != null)
                                || (node.Neighbor_M1plus1_M2 != null
                                    && node.Neighbor_M1plus1_M2minus1 == null)
                                || (node.Neighbor_M1plus1_M2 == null
                                    && node.Neighbor_M1plus1_M2minus1 != null)
                                || (node.Neighbor_M1_M2plus1 != null
                                    && node.Neighbor_M1minus1_M2plus1 == null)
                                || (node.Neighbor_M1_M2plus1 == null
                                    && node.Neighbor_M1minus1_M2plus1 != null)
                                || (node.Neighbor_M1_M2minus1 != null
                                    && node.Neighbor_M1plus1_M2minus1 == null)
                                || (node.Neighbor_M1_M2minus1 == null
                                    && node.Neighbor_M1plus1_M2minus1 != null)
                                || (node.Neighbor_M1minus1_M2 != null
                                    && node.Neighbor_M1minus1_M2plus1 == null)
                                || (node.Neighbor_M1minus1_M2 == null
                                    && node.Neighbor_M1minus1_M2plus1 != null))
                            {
                                node.rawCoefficient = 1.0;
                                node.IsSet = true;
                                node.IsSeedNode = true;
                                seedNode = node;
                                break;
                            }
                        }
                    }
                    if (seedNode != null)
                        break;
                }
            }

            public void CalculateRawCoefficients()
            {
                var passCount = 0;
                while (unprocessedNodeList.Count > 0
                    && passCount < 10)
                {
                    passCount++;
                    Console.WriteLine("Pass: " + passCount.ToString() + "; Remaining unprocessed: " + unprocessedNodeList.Count.ToString());
                    List<CBNode> skippedNodes = new List<CBNode>();
                    foreach (CBNode node in unprocessedNodeList)
                    {
                        if (!node.IsSet)
                        {
                            if (!node.CalculateRawCoefficient(j1: j1, j2: j2, j: j))
                            {
                                skippedNodes.Add(node);
                            }
                        }
                    }
                    unprocessedNodeList = skippedNodes;
                    DrawGrid();
                }
                Console.WriteLine("Finished calculating raw coefficients");
            }

            public void NormalizeCoefficients()
            {
                var total = 0.0;
                foreach (KeyValuePair<Tuple<int, int>, CBNode> kvp in grid)
                {
                    if (kvp.Value.m1 + kvp.Value.m2 == m)
                        total += (kvp.Value.rawCoefficient * kvp.Value.rawCoefficient);
                }
                foreach (KeyValuePair<Tuple<int, int>, CBNode> kvp in grid)
                {
                    if (kvp.Value.m1 + kvp.Value.m2 == m)
                    {
                        kvp.Value.normalizedCoefficient = kvp.Value.rawCoefficient / Math.Sqrt(total);
                        kvp.Value.IsNormalized = true;
                    }
                }
            }

            public void DrawGrid()
            {
                var strBldr = new StringBuilder();

                var maxCellStr = "-" + j.ToString() + "/2,-" + j.ToString() + "/2; Y";
                var cellLen = maxCellStr.Length;
                var rowLen = (cellLen + 3) * (2 * j + 1);
                Console.WriteLine(strBldr.Clear().Append(char.Parse("-"), rowLen));
                for (int m2 = j2; m2 >= -j2; m2-=2)
                {
                    for (int rowPass = 0; rowPass < 2; rowPass++)
                    {
                        for (int m1 = -j1; m1 <= j1; m1+=2)
                        {
                            var coord = new Tuple<int, int>(m1, m2);
                            if (grid.ContainsKey(coord))
                            {
                                if (rowPass == 0)
                                {
                                    var str = m1.ToString() + "/2," + m2.ToString() + "/2";
                                    if (grid[coord].IsSet)
                                        str += "; Y";
                                    Console.Write(str);
                                    var diff = maxCellStr.Length - str.Length;
                                    Console.Write(strBldr.Clear().Append(char.Parse(" "), diff));
                                    Console.Write(" | ");
                                }
                                else
                                {
                                    var coefStr = "";
                                    if (grid[coord].IsSet)
                                    {
                                        if (grid[coord].IsNormalized)
                                            coefStr = grid[coord].normalizedCoefficient.ToString("0.0000") + "(N)";
                                        else
                                            coefStr = grid[coord].rawCoefficient.ToString("0.0000");
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

        

        static void Main(string[] args)
        {
            bool continueProgram = true;
            while (continueProgram)
            {
                bool isValidInput = false;
                int numeratorJ1 = 0;
                int numeratorJ2 = 0;
                int numeratorJ = 0;
                int numeratorM = 0;
                int numeratorM1 = 0;
                int numeratorM2 = 0;
                while (!isValidInput)
                {
                    Console.WriteLine("J1 numerator (in half-integer steps):");
                    numeratorJ1 = int.Parse(Console.ReadLine());
                    Console.WriteLine("J2 numerator (in half-integer steps):");
                    numeratorJ2 = int.Parse(Console.ReadLine());
                    Console.WriteLine("J numerator (in half-integer steps):");
                    numeratorJ = int.Parse(Console.ReadLine());
                    Console.WriteLine("M numerator (in half-integer steps):");
                    numeratorM = int.Parse(Console.ReadLine());
                    Console.WriteLine("M1 numerator (in half-integer steps):");
                    numeratorM1 = int.Parse(Console.ReadLine());
                    Console.WriteLine("M2 numerator (in half-integer steps):");
                    numeratorM2 = int.Parse(Console.ReadLine());

                    if (Math.Abs(numeratorJ1 - numeratorJ2) > numeratorJ
                        || numeratorJ > numeratorJ1 + numeratorJ2)
                        Console.WriteLine("Parameters violate |j1 - j2| <= j <= j1 + j2");
                    else if (numeratorM1 + numeratorM2 != numeratorM)
                        Console.WriteLine("Parameters violate m1 + m2 = m");
                    else if (Math.Abs(numeratorM1) > numeratorJ1)
                        Console.WriteLine("Parameters violate |m1| <= j1");
                    else if (Math.Abs(numeratorM2) > numeratorJ2)
                        Console.WriteLine("Parameters violate |m2| <= j2");
                    else if (Math.Abs(numeratorM1 + numeratorM2) > numeratorJ)
                        Console.WriteLine("Parameters violate -j <= m1 + m2 <= j");
                    else if (Math.Abs(numeratorJ1 % 2) != Math.Abs(numeratorM1 % 2))
                        Console.WriteLine("Parameters violate j1 and m1 must both simultaneously be integers or half integers");
                    else if (Math.Abs(numeratorJ2 % 2) != Math.Abs(numeratorM2 % 2))
                        Console.WriteLine("Parameters violate j2 and m2 must both simultaneously be integers or half integers");
                    else
                        isValidInput = true;
                }
                

                CBScenario scenario = new CBScenario(j1: numeratorJ1, j2: numeratorJ2, j: numeratorJ, m: numeratorM);
                scenario.InitializeGrid();
                if (scenario.seedNode != null)
                    Console.WriteLine("Seed node set: m1: " + scenario.seedNode.m1.ToString() + "/2; m2: " + scenario.seedNode.m2.ToString() + "/2");
                else
                    Console.Write("-----SEED NODE NOT SET-----");

                scenario.CalculateRawCoefficients();
                scenario.NormalizeCoefficients();
                scenario.DrawGrid();

                var node = scenario.grid[new Tuple<int, int>(numeratorM1, numeratorM2)];
                Console.WriteLine("Requested coefficient: " + node.normalizedCoefficient.ToString("0.0000"));

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
