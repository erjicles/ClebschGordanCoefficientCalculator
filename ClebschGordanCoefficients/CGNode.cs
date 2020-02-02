using Radicals;
using Rationals;
using System;
using System.Globalization;
using System.Linq;

namespace ClebschGordanCoefficients
{
    public class CBNode
    {
        public Rational m1 { get; set; } = 0;
        public Rational m2 { get; set; } = 0;

        public Tuple<Rational, Rational> GridCoordinate { get; set; }
        public bool IsSet { get; set; } = false;
        public bool IsSeedNode { get; set; } = false;

        public RadicalSumRatio rawCoefficient { get; set; } = 0;
        public RadicalSumRatio normalizedCoefficient { get; set; } = 0;
        public int sign = 1;

        public NormalizationStatus status { get; set; } = NormalizationStatus.RAW;

        public enum NormalizationStatus
        {
            RAW,
            NORMALIZED,
            NORM_SQUARED
        }

        // References to neighbor nodes
        public CBNode n_m1_00 { get; set; }
        public CBNode n_00_m1 { get; set; }
        public CBNode n_00_p1 { get; set; }
        public CBNode n_p1_00 { get; set; }
        public CBNode n_p1_m1 { get; set; }
        public CBNode n_p1_p1 { get; set; }
        public CBNode n_m1_m1 { get; set; }
        public CBNode n_m1_p1 { get; set; }

        public CBNode(Rational m1, Rational m2)
        {
            this.m1 = m1;
            this.m2 = m2;
            this.GridCoordinate = new Tuple<Rational, Rational>(m1, m2);
        }

        public override string ToString()
        {
            if (status == NormalizationStatus.RAW)
                return rawCoefficient.ToString("R", CultureInfo.InvariantCulture) + " (Raw)";
            else if (status == NormalizationStatus.NORMALIZED)
                return normalizedCoefficient.ToString("R", CultureInfo.InvariantCulture);
            else if (status == NormalizationStatus.NORM_SQUARED)
                return (sign < 0 ? "-" : "") + "Sqrt(" + normalizedCoefficient.ToString("R", CultureInfo.InvariantCulture) + ")";
            else
                throw new Exception("Unknown status");
        }

        public bool CalculateRawCoefficient(Rational j1, Rational j2, Rational j)
        {
            // Check if we can calculate the raw coefficient
            if (IsSet)
                return false;

            //  x   O
            //      
            int calculationMethod = 0;
            if (n_m1_00 != null
                && n_m1_00.IsSet
                && n_00_m1 == null)
                calculationMethod = 1;

            //      O
            //      x
            if (n_m1_00 == null
                && n_00_m1 != null
                && n_00_m1.IsSet)
                calculationMethod = 2;

            //  x   O
            //      x
            if (n_m1_00 != null
                && n_m1_00.IsSet
                && n_00_m1 != null
                && n_00_m1.IsSet)
                calculationMethod = 3;

            //  
            //  O   x
            if (n_p1_00 != null
                && n_p1_00.IsSet
                && n_00_p1 == null)
                calculationMethod = 4;

            //  x
            //  O   
            if (n_p1_00 == null
                && n_00_p1 != null
                && n_00_p1.IsSet)
                calculationMethod = 5;

            //  x
            //  O   x
            if (n_p1_00 != null
                && n_p1_00.IsSet
                && n_00_p1 != null
                && n_00_p1.IsSet)
                calculationMethod = 6;

            //  O
            //  x   
            if (n_00_m1 != null
                && n_00_m1.IsSet
                && n_p1_m1 == null)
                calculationMethod = 7;

            //  O
            //      x
            if (n_00_m1 == null
                && n_p1_m1 != null
                && n_p1_m1.IsSet)
                calculationMethod = 8;

            //  O
            //  x   x
            if (n_00_m1 != null
                && n_00_m1.IsSet
                && n_p1_m1 != null
                && n_p1_m1.IsSet)
                calculationMethod = 9;

            //  
            //  x   O
            if (n_m1_00 != null
                && n_m1_00.IsSet
                && n_m1_p1 == null)
                calculationMethod = 10;

            //  x
            //      O
            if (n_m1_00 == null
                && n_m1_p1 != null
                && n_m1_p1.IsSet)
                calculationMethod = 11;

            //  x
            //  x   O
            if (n_m1_00 != null
                && n_m1_00.IsSet
                && n_m1_p1 != null
                && n_m1_p1.IsSet)
                calculationMethod = 12;

            //  O   x
            //      
            if (n_p1_00 != null
                && n_p1_00.IsSet
                && n_p1_m1 == null)
                calculationMethod = 13;

            //  O   
            //      x
            if (n_p1_00 == null
                && n_p1_m1 != null
                && n_p1_m1.IsSet)
                calculationMethod = 14;

            //  O   x
            //      x
            if (n_p1_00 != null
                && n_p1_00.IsSet
                && n_p1_m1 != null
                && n_p1_m1.IsSet)
                calculationMethod = 15;

            //      x
            //      O
            if (n_m1_p1 == null
                && n_00_p1 != null
                && n_00_p1.IsSet)
                calculationMethod = 16;

            //  x   
            //      O
            if (n_m1_p1 != null
                && n_m1_p1.IsSet
                && n_00_p1 == null)
                calculationMethod = 17;

            //  x   x
            //      O
            if (n_m1_p1 != null
                && n_m1_p1.IsSet
                && n_00_p1 != null
                && n_00_p1.IsSet)
                calculationMethod = 18;

            if (calculationMethod == 0)
                return false;

            // J+:
            // Type 0: <m1, m2; j, m + 1> = {sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m> + sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j - m)(j + m + 1)]
            // Type 4: <m1 - 1, m2; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
            // Type 5: <m1, m2 - 1; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j2 - m2 + 1)(j2 + m2)]

            // J-:
            // Type 1: <m1, m2; j, m - 1> = {sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m> + sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j + m)(j - m + 1)]
            // Type 3: <m1 + 1, m2; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
            // Type 2: <m1, m2 + 1; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j2 + m2 + 1)(j2 - m2)]
            int[] type0 = new int[] { 1, 2, 3 };
            int[] type1 = new int[] { 4, 5, 6 };
            int[] type2 = new int[] { 7, 8, 9 };
            int[] type3 = new int[] { 10, 11, 12 };
            int[] type4 = new int[] { 13, 14, 15 };
            int[] type5 = new int[] { 16, 17, 18 };

            var m = m1 + m2;
            if (type0.Contains(calculationMethod))
            {
                //  x   O
                //      x
                // J+:
                // Type 0: <m1, m2; j, m + 1> = {sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m> + sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j - m)(j + m + 1)]
                //  =>                      c = {                        a1 * c_m1_00           +                         a2 * c_00_m1          } / a0
                m -= 1;
                RadicalSumRatio c_m1_00 = n_m1_00 != null ? n_m1_00.rawCoefficient : 0;
                RadicalSumRatio c_00_m1 = n_00_m1 != null ? n_00_m1.rawCoefficient : 0;
                var a1 = new Radical((j1 - m1 + 1) * (j1 + m1));
                var a2 = new Radical((j2 - m2 + 1) * (j2 + m2));
                var a0 = new Radical((j - m) * (j + m + 1));

                rawCoefficient =
                    ((a1 * c_m1_00) + (a2 * c_00_m1)) / a0;
            }
            else if (type1.Contains(calculationMethod))
            {
                //  x
                //  O   x
                // J-:
                // Type 1: <m1, m2; j, m - 1> = {sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m> + sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j + m)(j - m + 1)]
                //  =>                      c = {                        a1 * c_p1_00           +                         a2 * c_00_p1          } / a0
                m += 1;
                RadicalSumRatio c_p1_00 = n_p1_00 != null ? n_p1_00.rawCoefficient : 0;
                RadicalSumRatio c_00_p1 = n_00_p1 != null ? n_00_p1.rawCoefficient : 0;
                Radical a1 = new Radical((j1 + m1 + 1) * (j1 - m1));
                Radical a2 = new Radical((j2 + m2 + 1) * (j2 - m2));
                Radical a0 = new Radical((j + m) * (j - m + 1));
                rawCoefficient =
                    ((a1 * c_p1_00) + (a2 * c_00_p1)) / a0;
            }
            else if (type2.Contains(calculationMethod))
            {
                //  O
                //  x   x
                // J-:
                // Type 2: <m1, m2 + 1; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j1 + m1 + 1)(j1 - m1)]<m1 + 1, m2; j, m>} / sqrt[(j2 + m2 + 1)(j2 - m2)]
                //  =>                      c = {                    a1 * c_00_m1           -                         a2 * c_p1_m1          } / a0
                var M2 = m2 - 1;
                m = m1 + M2;
                m += 1;
                RadicalSumRatio c_00_m1 = n_00_m1 != null ? n_00_m1.rawCoefficient : 0;
                RadicalSumRatio c_p1_m1 = n_p1_m1 != null ? n_p1_m1.rawCoefficient : 0;
                Radical a1 = new Radical((j + m) * (j - m + 1));
                Radical a2 = new Radical((j1 + m1 + 1) * (j1 - m1));
                Radical a0 = new Radical((j2 + M2 + 1) * (j2 - M2));
                rawCoefficient =
                    ((a1 * c_00_m1) - (a2 * c_p1_m1)) / a0;
            }
            else if (type3.Contains(calculationMethod))
            {
                //  x
                //  x   O
                // J-:
                // Type 3: <m1 + 1, m2; j, m> = {sqrt[(j + m)(j - m + 1)]<m1, m2; j, m - 1> - sqrt[(j2 + m2 + 1)(j2 - m2)]<m1, m2 + 1; j, m>} / sqrt[(j1 + m1 + 1)(j1 - m1)]
                //          =>              c = {                    a1 * c_m1_00           -                         a2 * c_m1_p1          } / a0;
                var M1 = m1 - 1;
                m = M1 + m2;
                m += 1;
                RadicalSumRatio c_m1_00 = n_m1_00 != null ? n_m1_00.rawCoefficient : 0;
                RadicalSumRatio c_m1_p1 = n_m1_p1 != null ? n_m1_p1.rawCoefficient : 0;
                Radical a1 = new Radical((j + m) * (j - m + 1));
                Radical a2 = new Radical((j2 + m2 + 1) * (j2 - m2));
                Radical a0 = new Radical((j1 + M1 + 1) * (j1 - M1));
                rawCoefficient =
                    ((a1 * c_m1_00) - (a2 * c_m1_p1)) / a0;
            }
            else if (type4.Contains(calculationMethod))
            {
                //  O   x
                //      x
                // J+:
                // Type 4: <m1 - 1, m2; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j2 - m2 + 1)(j2 + m2)]<m1, m2 - 1; j, m>} / sqrt[(j1 - m1 + 1)(j1 + m1)]
                //  =>                      c = {                    a1 * c_p1_00           -                         a2 * c_p1_m1          } / a0
                var M1 = m1 + 1;
                m = M1 + m2;
                m -= 1;
                RadicalSumRatio c_p1_00 = n_p1_00 != null ? n_p1_00.rawCoefficient : 0;
                RadicalSumRatio c_p1_m1 = n_p1_m1 != null ? n_p1_m1.rawCoefficient : 0;
                Radical a1 = new Radical((j - m) * (j + m + 1));
                Radical a2 = new Radical((j2 - m2 + 1) * (j2 + m2));
                Radical a0 = new Radical((j1 - M1 + 1) * (j1 + M1));
                rawCoefficient =
                    ((a1 * c_p1_00) - (a2 * c_p1_m1)) / a0;
            }
            else if (type5.Contains(calculationMethod))
            {
                //  x   x
                //      O
                // J+:
                // Type 5: <m1, m2 - 1; j, m> = {sqrt[(j - m)(j + m + 1)]<m1, m2; j, m + 1> - sqrt[(j1 - m1 + 1)(j1 + m1)]<m1 - 1, m2; j, m>} / sqrt[(j2 - m2 + 1)(j2 + m2)]
                //  =>                      c = {                    a1 * c_00_p1           -                         a2 * c_m1_p1          } / a0
                var M2 = m2 + 1;
                m = m1 + M2;
                m -= 1;
                RadicalSumRatio c_00_p1 = n_00_p1 != null ? n_00_p1.rawCoefficient : 0;
                RadicalSumRatio c_m1_p1 = n_m1_p1 != null ? n_m1_p1.rawCoefficient : 0;
                Radical a1 = new Radical((j - m) * (j + m + 1));
                Radical a2 = new Radical((j1 - m1 + 1) * (j1 + m1));
                Radical a0 = new Radical((j2 - M2 + 1) * (j2 + M2));
                rawCoefficient =
                    ((a1 * c_00_p1) - (a2 * c_m1_p1)) / a0;
            }
            else
            {
                throw new Exception("Invalid calculation method: " + calculationMethod.ToString());
            }

            IsSet = true;


            return true;
        }
    }
}
