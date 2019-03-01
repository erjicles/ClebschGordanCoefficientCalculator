using Radicals;
using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClebschGordanCoefficients
{
    class CBScenario
    {
        public Rational j1 { get; set; }
        public Rational j2 { get; set; }
        public Rational j { get; set; }
        public Rational m { get; set; }

        public Dictionary<Tuple<Rational, Rational>, CBNode> grid { get; set; }
        public List<CBNode> unprocessedNodeList { get; set; }
        public CBNode seedNode { get; set; }

        public CBScenario(
            Rational j1, 
            Rational j2, 
            Rational j, 
            Rational m)
        {
            this.j1 = j1;
            this.j2 = j2;
            this.j = j;
            this.m = m;
        }

        public void InitializeGrid()
        {
            grid = new Dictionary<Tuple<Rational, Rational>, CBNode>();
            unprocessedNodeList = new List<CBNode>();
            // Create nodes
            for (Rational m1 = -j1; m1 <= j1; m1 += 1)
            {
                for (Rational m2 = -j2; m2 <= j2; m2 += 1)
                {
                    if (Rational.Abs(m1 + m2) <= j
                        && Rational.Abs(m1) <= j1
                        && Rational.Abs(m2) <= j2)
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
            for (Rational m1 = -j1; m1 <= j1; m1 += 1)
            {
                for (Rational m2 = -j2; m2 <= j2; m2 += 1)
                {
                    var coord = new Tuple<Rational, Rational>(m1, m2);
                    if (grid.ContainsKey(coord))
                    {
                        var node = grid[coord];

                        var c_m1_00 = new Tuple<Rational, Rational>(m1 - 1, m2);
                        var c_00_m1 = new Tuple<Rational, Rational>(m1, m2 - 1);
                        var c_00_p1 = new Tuple<Rational, Rational>(m1, m2 + 1);
                        var c_p1_00 = new Tuple<Rational, Rational>(m1 + 1, m2);
                        var c_p1_m1 = new Tuple<Rational, Rational>(m1 + 1, m2 - 1);
                        var c_p1_p1 = new Tuple<Rational, Rational>(m1 + 1, m2 + 1);
                        var c_m1_m1 = new Tuple<Rational, Rational>(m1 - 1, m2 - 1);
                        var c_m1_p1 = new Tuple<Rational, Rational>(m1 - 1, m2 + 1);

                        if (grid.ContainsKey(c_m1_00))
                            node.n_m1_00 = grid[c_m1_00];
                        if (grid.ContainsKey(c_00_m1))
                            node.n_00_m1 = grid[c_00_m1];
                        if (grid.ContainsKey(c_00_p1))
                            node.n_00_p1 = grid[c_00_p1];
                        if (grid.ContainsKey(c_p1_00))
                            node.n_p1_00 = grid[c_p1_00];
                        if (grid.ContainsKey(c_p1_m1))
                            node.n_p1_m1 = grid[c_p1_m1];
                        if (grid.ContainsKey(c_p1_p1))
                            node.n_p1_p1 = grid[c_p1_p1];
                        if (grid.ContainsKey(c_m1_m1))
                            node.n_m1_m1 = grid[c_m1_m1];
                        if (grid.ContainsKey(c_m1_p1))
                            node.n_m1_p1 = grid[c_m1_p1];
                    }
                }
            }
            // Set the seed node
            // Looking for a node that only has one non-null neighbor as part of a triangle
            // Start by trying the maximal m
            for (Rational m1 = j1; m1 >= -j1; m1 -= 1)
            {
                for (Rational m2 = j2; m2 >= -j2; m2 -= 1)
                {
                    var coord = new Tuple<Rational, Rational>(m1, m2);
                    if (grid.ContainsKey(coord))
                    {
                        var node = grid[coord];
                        if ((node.n_m1_00 != null
                                && node.n_00_m1 == null)
                            || (node.n_m1_00 == null
                                && node.n_00_m1 != null)
                            || (node.n_p1_00 != null
                                && node.n_00_p1 == null)
                            || (node.n_p1_00 == null
                                && node.n_00_p1 != null)
                            || (node.n_p1_00 != null
                                && node.n_p1_m1 == null)
                            || (node.n_p1_00 == null
                                && node.n_p1_m1 != null)
                            || (node.n_00_p1 != null
                                && node.n_m1_p1 == null)
                            || (node.n_00_p1 == null
                                && node.n_m1_p1 != null)
                            || (node.n_00_m1 != null
                                && node.n_p1_m1 == null)
                            || (node.n_00_m1 == null
                                && node.n_p1_m1 != null)
                            || (node.n_m1_00 != null
                                && node.n_m1_p1 == null)
                            || (node.n_m1_00 == null
                                && node.n_m1_p1 != null))
                        {
                            node.rawCoefficient = 1;
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
            }
            Console.WriteLine("Finished calculating raw coefficients");
        }

        public void NormalizeCoefficients()
        {
            CompositeRadicalRatio total = 0;
            foreach (KeyValuePair<Tuple<Rational, Rational>, CBNode> kvp in grid)
            {
                if (kvp.Value.m1 + kvp.Value.m2 == m)
                    total += (kvp.Value.rawCoefficient * kvp.Value.rawCoefficient);
            }
            foreach (KeyValuePair<Tuple<Rational, Rational>, CBNode> kvp in grid)
            {
                if (kvp.Value.m1 + kvp.Value.m2 == m)
                {
                    if (total.IsRational())
                    {
                        var normalizer = new BasicRadical(total.ToRational());
                        kvp.Value.normalizedCoefficient = kvp.Value.rawCoefficient / normalizer;
                        kvp.Value.status = CBNode.NormalizationStatus.NORMALIZED;
                    }
                    else
                    {
                        kvp.Value.normalizedCoefficient = (kvp.Value.rawCoefficient * kvp.Value.rawCoefficient) / total;
                        kvp.Value.sign = kvp.Value.rawCoefficient >= 0 ? 1 : -1;
                        kvp.Value.status = CBNode.NormalizationStatus.NORM_SQUARED;
                    }
                }
            }
        }

        
    }
}
