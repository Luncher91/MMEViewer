using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMEData.MMEChannel;

namespace MMEDataMath
{
    public static class CFCFilters
    {
        public static void CFCFilter(this MMEChannel source, MMEFilter filter)
        {
            source.Values = ApplyFilter(source.Values, source.SampleInterval, filter);
        }

        public static void CFCFilter(this MMEChannel source, int filterClass)
        {
            source.Values = ApplyFilter(source.Values, source.SampleInterval, GetFilterEnum(filterClass));
        }

        private static List<double> ApplyFilter(List<double> values, double sampleInterval, MMEFilter filterClass)
        {
            if (filterClass == MMEFilter.None)
                return values;

            double[] filteredValues = values.ToArray();

            filteredValues = SAEJ211FilterValues(filteredValues, sampleInterval, filterClass);

            return filteredValues.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputValues"></param>
        /// <param name="samplerate">sampling rate in Hz</param>
        /// <param name="CFC"></param>
        /// <returns></returns>
        private static double[] SAEJ211FilterValues(double[] inputValues, double sampleInterval, MMEFilter filterClass)
        {
            // TODO: refactor this method; it is way too long and complicated!
            if (filterClass == MMEFilter.None)
                return inputValues;

            int CFC = GetCFCNumber(filterClass);
            double[] output = new double[inputValues.Length];
            double SQRTTWO = Math.Sqrt(2);
            double wd, wa;
            double a0, a1, a2, b1, b2;
            double inp1 = 0, inp2 = 0, out1 = 0, out2 = 0;

            wd = 2 * Math.PI * CFC / 0.6 * 1.25;
            wa = Math.Tan(wd * sampleInterval / 2.0);

            double sqrtTwoWa = SQRTTWO * wa;
            double waSqrd = wa * wa;
            double denominator = 1 + sqrtTwoWa + waSqrd;

            a0 = waSqrd / denominator;
            a1 = 2 * a0;
            a2 = a0;

            b1 = -2 * (1 - waSqrd) / denominator;
            b2 = (1 - sqrtTwoWa + waSqrd) / denominator;

            inp1 = inputValues[1];
            inp2 = inputValues[0];

            // forward
            for (int i = 2; i < output.Length; i++)
            {
                double inp0 = inputValues[i];
                double newValue = a0 * inp0 + a1 * inp1 + a2 * inp2 - b1 * out1 - b2 * out2;

                output[i] = newValue;

                out2 = out1;
                out1 = newValue;

                inp2 = inp1;
                inp1 = inp0;
            }

            double[] outputSecondRound = new double[output.Length];
            // reset
            inp1 = output[1]; inp2 = output[0]; out1 = 0; out2 = 0;

            // backwards
            for (int i = output.Length - 3; i >= 0; i--)
            {
                double inp0 = output[i];
                double newValue = a0 * inp0 + a1 * inp1 + a2 * inp2 - b1 * out1 - b2 * out2;

                outputSecondRound[i] = newValue;

                out2 = out1;
                out1 = newValue;

                inp2 = inp1;
                inp1 = inp0;
            }

            return outputSecondRound;
        }
    }
}
