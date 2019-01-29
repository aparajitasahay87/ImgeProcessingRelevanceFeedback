using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class RelevantImage
    {
        public string imageID;
        public bool ImageFound = false;
        public List<double> GaussianValue = new List<double>();

        //public double getGaussianValue(int index)
        //{
        //    double binValue = 0.0;
        //    foreach (KeyValuePair<int, double> pair in histogramBinRfMethod)
        //    {
        //        if (pair.Key == index)
        //        {
        //            binValue = pair.Value;
        //            break;
        //        }
        //    }
        //    return binValue;
        //}
    }
}
