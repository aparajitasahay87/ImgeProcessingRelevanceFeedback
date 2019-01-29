using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class HistogramBinRFMethod
    {
        int histogramSize = 0;
        public string imageID;
        Dictionary<int, double> histogramBinRfMethod = new Dictionary<int, double>();
        public void setHistogramBinRfMethod(Dictionary<int, int> intensity, Dictionary<int, int> colorCode, int imageSize)
        {
            foreach (KeyValuePair<int, int> pair in colorCode)
            {
                int binValueColorCode = pair.Value;
                double newBinValueRfFromColorCode = 0.0;
                newBinValueRfFromColorCode = (double)binValueColorCode / imageSize;
                if (histogramSize < 89)
                {
                histogramBinRfMethod.Add(histogramSize, newBinValueRfFromColorCode);
                histogramSize++;
                }
            }
            foreach (KeyValuePair<int, int> pair in intensity)
            {
                int binValueIntensity = pair.Value;
                double newBinValueRfFromIntensity = 0.0;
                newBinValueRfFromIntensity = (double)binValueIntensity / imageSize;
                if (histogramSize < 89)
                {
                    histogramBinRfMethod.Add(histogramSize, newBinValueRfFromIntensity);
                    histogramSize++;
                }
            }
            
        }

        public Dictionary<int, double> getHistogramBinRfMethod()
        {
            return histogramBinRfMethod;
        }

        public double getHistogramBinRfMethod(int index)
        {
            double binValue = 0.0;
            foreach(KeyValuePair<int, double> pair in histogramBinRfMethod)
            {
                if(pair.Key == index)
                {
                    binValue = pair.Value;
                    break;
                }
            }
            return binValue;
        }
    }
}
