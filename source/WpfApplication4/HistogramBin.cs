using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class HistogramBin
    {
        private string filepathId;
        //private int numberOfPixel = 0;
        int totalPixelInImage = 0;
        private Dictionary<int, int> histogramBinIntensityMethod = new Dictionary<int, int>();
        //bool selectedImage = false;
        private Dictionary<int, int> histogramBinColorCodeMethod = new Dictionary<int, int>();
        int histogramBinSize = 10;
        public void initializeHistogramIntensityMethod()
        {
            for (int i = 0; i < 25; i++)
            {
                histogramBinIntensityMethod.Add(i, 0);
            }
        }
        public void setNumberOfPixelImage(int totalPixel)
        {
            totalPixelInImage = totalPixel;
            //numberOfPixel++;
        }

        public int getNumberOfPixelImage()
        {
            //return numberOfPixel;
            return totalPixelInImage;
        }

        public void setImageId(string filePath)
        {
            filepathId = filePath;
        }
        public string getImageId()
        {
            return filepathId;
        }
        public Dictionary<int, int> getValueHistogram()
        {
            return histogramBinIntensityMethod;
        }
        public void addIntensityToBin(double intensity)
        {
            foreach (KeyValuePair<int, int> pair in histogramBinIntensityMethod)
            {
                int startIndex = pair.Key * histogramBinSize;
                int endIndex = (pair.Key * histogramBinSize) + histogramBinSize;
                //int intensity1 = (int)intensity;
                if (intensity >= startIndex && intensity < endIndex)
                {
                    int countBin = pair.Value;
                    //histogramBinIntensityMethod.Add(pair.Key,countBin);
                    countBin++;
                    histogramBinIntensityMethod[pair.Key] = countBin;
                    break;
                }
            }
        }
        public void initializehistogramColorCodeMethod()
        {
            for (int i = 0; i < 64; i++)
            {
                histogramBinColorCodeMethod.Add(i, 0);
            }
        }
        public void addElementHistogramColorCodeMethod(int index)
        {
            foreach (KeyValuePair<int, int> pair in histogramBinColorCodeMethod)
            {
                if(pair.Key==index)
                {
                    int countBin = pair.Value;
                    //histogramBinIntensityMethod.Add(pair.Key,countBin);
                    countBin++;
                    histogramBinColorCodeMethod[pair.Key] = countBin;
                    break;
                }
            }
        }

        public Dictionary<int, int> getValueColorCodeHistogram()
        {
            return histogramBinColorCodeMethod;
        }
    }
}
