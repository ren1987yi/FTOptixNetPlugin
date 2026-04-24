using MathNet.Numerics.Distributions;

namespace TimeSeriesAlgorithms
{
    public class SenMK
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha"></param>
        /// <param name="Z"></param>
        /// <param name="T"></param>
        public static void Analyze(List<double> data, double alpha, out double Z, out TrendSign_TE T)
        {
            if (alpha == 0)
            {
                alpha = 0.05;
            }
            var S = ComputeS(data);
            var varS = ComputeVariance(data);
            Z = ComputeZ(S, varS);
            T = TrendSignificance(Z, alpha); //趋势
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha"></param>
        /// <param name="predict_distance"></param>
        /// <param name="Z"></param>
        /// <param name="T"></param>
        /// <param name="slop"></param>
        /// <param name="intercept"></param>
        /// <param name="predict_values"></param>
        public static void AnalyzeAndPredict(List<double> data, double alpha, int predict_distance, out double Z, out TrendSign_TE T, out double slop,out double intercept, out List<double> predict_values)
        {
            slop = CalculateSensSlope(data);
            intercept = LinearFit(data,slop);


            Analyze(data, alpha, out Z, out T);

            var p0 = data.Last();

            predict_values = new List<double>();
            predict_values.Add(intercept);
            predict_values.Add(intercept + slop * (data.Count +  predict_distance));
            
        }



        // 计算 Mann-Kendall 检验的 S 值
        private static int ComputeS(List<double> data)
        {
            int n = data.Count;
            int S = 0;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (data[j] > data[i])
                        S += 1;
                    else if (data[j] < data[i])
                        S -= 1;
                }
            }

            return S;
        }

        // 计算 S 的方差
        private static double ComputeVariance(List<double> data)
        {
            int n = data.Count;
            return n * (n - 1) * (2 * n + 5) / 18.0;
        }

        // 计算 Z 值
        private static double ComputeZ(int S, double varS)
        {
            if (S > 0)
                return (S - 1) / Math.Sqrt(varS);
            else if (S == 0)
                return 0;
            else
                return (S + 1) / Math.Sqrt(varS);
        }

        // 判定趋势显著性
        private static TrendSign_TE TrendSignificance(double Z, double alpha = 0.05)
        {
            // For a two-tailed test, divide alpha by 2
            double alphaHalf = alpha / 2;

            // Calculate the critical z-values using the standard normal distribution
            // Two-tailed test critical values

            double zCriticalLower = Normal.InvCDF(0, 1, alphaHalf); // Lower tail
            double zCriticalUpper = Normal.InvCDF(0, 1, 1 - alphaHalf); // Upper tail

            double Z_crit = Math.Abs(zCriticalUpper); // 双尾 alpha=0.05 时的临界值


            if (Z > Z_crit)
                return TrendSign_TE.Rise;
            else if (Z < -Z_crit)
                return TrendSign_TE.Fall;
            else
                return TrendSign_TE.None;
        }


        // Calculates the Sen's Slope of a dataset
        private static double CalculateSensSlope(List<double> data)
        {
            int n = data.Count;
            List<double> slopes = new List<double>();

            // Generate slopes for all pairs (i, j) where i < j
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    slopes.Add((data[j] - data[i]) / (j - i));
                }
            }

            // Calculate and return the median slope
            slopes.Sort();
            int count = slopes.Count;
            if (count % 2 == 1)
            {
                return slopes[count / 2];
            }
            else
            {
                return (slopes[(count / 2) - 1] + slopes[count / 2]) / 2.0;
            }
        }


        // 计算 Mann-Kendall 检验的 S 值
        private static double LinearFit(List<double> yData,double slop)
        {
            int n  = yData.Count;
            
            double sumX = n * (n+1) /2;

            double sumY = yData.Sum();
            double intercept = (sumY - slop * sumX) / n;
            return intercept;
        }

    }
}
