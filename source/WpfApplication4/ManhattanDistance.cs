using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class ManhattanDistance
    {
        //Final result of manhattan distance
        List<ResultCBIR> DistancebetweenImage = new List<ResultCBIR>();
        public List<ResultCBIR> computeManhattanDistance(string selecteImageFilePath, List<HistogramBin> histogramBinInensity,string method)
        {
            //Retirieve selected image histogram value
            Dictionary<int, int> selectedImageHistogram = new Dictionary<int, int>();
            List<double> selectedImage = new List<double>();
            int totalPixelSelectedImage = 0;
            foreach (HistogramBin image in histogramBinInensity)
            {
                string imageid = image.getImageId();
                if(selecteImageFilePath==null)
                {
                    
                    return null;
                }
                if (new Uri(imageid) == new Uri(selecteImageFilePath))
                {
                    totalPixelSelectedImage = image.getNumberOfPixelImage();
                    //selected_image histogram
                    if (method == "intensity")
                    {
                        selectedImageHistogram.Clear();
                        selectedImageHistogram = image.getValueHistogram();
                    }
                    else
                    {
                        selectedImageHistogram.Clear();
                        selectedImageHistogram = image.getValueColorCodeHistogram();
                    }
                    foreach (KeyValuePair<int, int> pair in selectedImageHistogram)
                    {
                        selectedImage.Add(pair.Value);
                    }
                    break;
                }
            }

            foreach (HistogramBin images in histogramBinInensity)
            {
                int i = 0;
                double totalDistance = 0;
                ResultCBIR dis = new ResultCBIR();
                Dictionary<int, int> image = new Dictionary<int, int>();
                int condition = 0;
                if (method == "intensity")
                {
                     image.Clear();
                     image = images.getValueHistogram();
                     condition = 25;
                }
                else
                {
                    image.Clear();
                    image = images.getValueColorCodeHistogram();
                    condition = 64;
                }
                int totalPixel = images.getNumberOfPixelImage();
                string imageID = images.getImageId();
                dis.imageid = imageID;
                
                while (i < condition)
                {
                    //look for key in dictionary and store the value
                    double numberOfPixelInBin = image[i];
                    double argument1 = numberOfPixelInBin / totalPixel;

                    double numberOfPixelInSelectedImageBin = selectedImage.ElementAt(i);
                    double argument2 = numberOfPixelInSelectedImageBin / totalPixelSelectedImage;

                    double distance = Math.Abs(argument2 - argument1);

                    totalDistance = totalDistance + distance;
                    i++;
                }

                dis.totaldistance = totalDistance;
                DistancebetweenImage.Add(dis);
            }
            //Orderby 
            DistancebetweenImage = DistancebetweenImage.OrderBy(i => i.totaldistance).ToList();
            return DistancebetweenImage;

        }
    }
}
