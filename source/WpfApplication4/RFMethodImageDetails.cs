using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class RFMethodImageDetails
    {
        public string imageID;
        public int binNumber = 0;
        public List<double> GaussianNormalizedWeight = new List<double>();
        public Dictionary<int, double> ImageHistogram = new Dictionary<int, double>();
    }
}
