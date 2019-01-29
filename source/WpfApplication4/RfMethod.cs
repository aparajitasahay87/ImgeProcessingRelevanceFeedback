using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class RfMethod
    {
        List<double> meanValue = new List<double>(89);
        //Initialize the list to zero value
        List<double> standardDeviation = new List<double>();
        int currentFeature = 0;
        int currentImageId = 0;
        //To keep track of iteration number for computing RF mehtod
        private int numberOfIteration = 0;
        //int totalNumberOfBin = 89;
        //double sumUpdatedWeight = 0.0;
        string queryImage=null;
        List<ResultCBIR> DistancebetweenImage = new List<ResultCBIR>();
        List<RFMethodFeatureDetails> featureMeanValue = new List<RFMethodFeatureDetails>();
        List<HistogramBinRFMethod> allImageHistogramRfMethod = new List<HistogramBinRFMethod>();

        List<RFMethodFeatureDetails> allFeature = new List<RFMethodFeatureDetails>();
        List<RFMethodImageDetails> allImageDetails = new List<RFMethodImageDetails>();
        //Storing normalized weight of query image
        List<double> selectedImage = new List<double>();
        //List of relevent images gaussian value 
        List<RelevantImage> relevantImageHistogram = new List<RelevantImage>();
        string methodName=null;

        public void setMethodName(string name)
        {
            methodName = name;
        }
        //Create histogram for RF method 
        public List<HistogramBinRFMethod> createHistogramRfMethod(List<HistogramBin> imageDetails)
        {
            foreach(HistogramBin img in imageDetails)
            {
                HistogramBinRFMethod bin = new HistogramBinRFMethod();
                int imageSize= img.getNumberOfPixelImage();
                Dictionary<int,int> intensity =img.getValueHistogram();
                string imageID = img.getImageId();
                Dictionary<int, int> colorCode = img.getValueColorCodeHistogram();
                //Call method to create histogram for RF method for each image. Add histogram of color code and intensity 
                bin.setHistogramBinRfMethod(intensity,colorCode,imageSize);
                bin.imageID = imageID;
                allImageHistogramRfMethod.Add(bin);
            }
            return allImageHistogramRfMethod;
        }

        //From outside this class user will send the rfmethodHistogram and this function will set the value of sent histogram 
        public void setallImageHistogramRfMethod(List<HistogramBinRFMethod> ImageHistogram)
        {
            allImageHistogramRfMethod = ImageHistogram;
        }
        //user will send the query image and it will be used for further computation in calculating ditance
        public void setQueryImage(string returnedQueryImage)
        {
            queryImage = returnedQueryImage;
        }
        private double calculateMeanValue()
        {
            if (methodName == "colorCodeIntensity")
            {
                double meanValue = 0.0;
                foreach (HistogramBinRFMethod rf in allImageHistogramRfMethod)
                {
                    meanValue += rf.getHistogramBinRfMethod(currentFeature);
                }
                meanValue = (double)meanValue / 100;
                return meanValue;
            }
            else
            {
                //look for relevantIMage gaussianValue only to create feature matrix
                double meanValue = 0.0;
                foreach (RelevantImage rf in relevantImageHistogram)
                {
                    meanValue += rf.GaussianValue.ElementAt(currentFeature);
                    //meanValue += rf.getHistogramBinRfMethod(currentFeature);
                }
                meanValue = (double)meanValue / relevantImageHistogram.Count;
                return meanValue;
            }
        }

        private double computeStandardDeviation(double average)
        {
            if (methodName == "colorCodeIntensity")
            {
                double intermediateResult = 0.0;
                //double standardDeviation = 0.0;
                //standard deviation formula
                //SQRT((POWER(C2-C7,2)+POWER(C3-C7,2)+POWER(C4-C7,2)+POWER(C5-C7,2))/(4-1))
                foreach (HistogramBinRFMethod rf in allImageHistogramRfMethod)
                {
                    double imageBinValue = rf.getHistogramBinRfMethod(currentFeature);
                    intermediateResult += Math.Pow((imageBinValue - average), 2);
                }
                double deviation = (double)intermediateResult / 99;
                double standardDeviation = Math.Sqrt(deviation);
                return standardDeviation;
            }
            else
            {
                double intermediateResult = 0.0;
                //double standardDeviation = 0.0;
                //standard deviation formula
                //SQRT((POWER(C2-C7,2)+POWER(C3-C7,2)+POWER(C4-C7,2)+POWER(C5-C7,2))/(4-1))
                foreach (RelevantImage rf in relevantImageHistogram)
                {
                    double imageBinValue = rf.GaussianValue.ElementAt(currentFeature);
                    intermediateResult += Math.Pow((imageBinValue - average), 2);
                }
                double deviation = (double)intermediateResult /( relevantImageHistogram.Count - 1) ;
                double standardDeviation = Math.Sqrt(deviation);
                return standardDeviation;
            }
        }

        private void computeUpdatedWeight(double minValue)
        {
            double updateWeight = 0.0;
            foreach(RFMethodFeatureDetails feature in allFeature)
            {
                if(feature.standardDeviation==0)
                {
                    if(feature.meanValue==0)
                    {
                        updateWeight = 0.0;
                    }
                    else
                    {
                        double value = (double)0.5 * minValue;
                        updateWeight = (double)1 / value;
                    }
                }
                else
                {
                    double standardDeviation = feature.standardDeviation;
                    updateWeight = (double)1 / standardDeviation;
                }
                feature.updatedWeight = updateWeight;
            }
            
        }

        public double computeSumUpdatedWeight()
        {
            double sumUpdatedWeight = 0.0;
            foreach(RFMethodFeatureDetails feature in allFeature)
            {
                sumUpdatedWeight+=feature.updatedWeight;
            }
            return sumUpdatedWeight;
        }
        //In each iteration for each feature calculate mean value,standard deviation, weight and updated weight
        public void addFeatureValues()
        {
            //ADD average value and standard deviation of each bin
            while (currentFeature < 89)
            {
                RFMethodFeatureDetails feature = new RFMethodFeatureDetails();
                //1) compute mean value 
                double average = calculateMeanValue();
                feature.meanValue = average;
                //2)compute standard deviation 
                double standardDeviation = computeStandardDeviation(average);
                feature.standardDeviation = standardDeviation;
                //feature.updatedWeight = computeUpdatedWeight(standardDeviation,average);
                //ID assigned to each bin from 1-89
                feature.featureID = currentFeature;
                //Add each feature details in one place i.e. List of RFMethodFeatureDetails type
                allFeature.Add(feature);
                currentFeature++;
            }
             
        }
        //After standarddeviation and mean value are calculted for each bin then
       

        public void computeGaussianNormalizedWeight(double minValue)
        {
            while (currentImageId < 100)
            {
                Dictionary<int, double> imageHistogram = allImageHistogramRfMethod.ElementAt(currentImageId).getHistogramBinRfMethod();
                string imageID = allImageHistogramRfMethod.ElementAt(currentImageId).imageID;                
                //new IMageDetails for each image
                RFMethodImageDetails imageDetails = new RFMethodImageDetails();
                //Every image has a unique id - fileapth of image 
                imageDetails.imageID = imageID;
                //Every image has a histogram 
                imageDetails.ImageHistogram = imageHistogram;
                foreach (KeyValuePair<int, double> pair in imageHistogram)
                {
                    double gaussianNormalizedWeight = 0.0;
                    double meanValue = 0.0;
                    double standardDeviation = 0.0;
                    int binNumber = pair.Key;
                    RFMethodFeatureDetails featureValues = allFeature.ElementAt(binNumber);
                    meanValue=featureValues.meanValue;
                    standardDeviation = featureValues.standardDeviation;
                    if (standardDeviation == 0)
                    {
                        //check mean value
                        if (meanValue == 0)
                        {
                            gaussianNormalizedWeight = 0.0;//set gaussiannormalized weight to 0
                            imageDetails.GaussianNormalizedWeight.Add(gaussianNormalizedWeight);
                        }
                        else
                        {
                            standardDeviation = 0.5 * minValue;
                            gaussianNormalizedWeight=(double)((pair.Value - meanValue) / standardDeviation);
                            imageDetails.GaussianNormalizedWeight.Add(gaussianNormalizedWeight);
                        }
                    }
                    else
                    {
                        gaussianNormalizedWeight = (double)((pair.Value - meanValue) / standardDeviation);
                        //Each image has 89 Gaussiannormalized weight(computed only once) for each bin, adding one item in a list at a time
                        imageDetails.GaussianNormalizedWeight.Add(gaussianNormalizedWeight);
                    }
                }
                allImageDetails.Add(imageDetails);
                currentImageId++;
            }
        }

        public List<RFMethodImageDetails> getGaussianValueImageDetails()
        {
            return allImageDetails;
        }
        private double checkMinStandardDeviation()
        {
            double minValue=int.MaxValue;
            foreach(RFMethodFeatureDetails bin in allFeature)
            {
                if(minValue>bin.standardDeviation && bin.standardDeviation>0)
                {
                    minValue = bin.standardDeviation;     
                }
            }
            return minValue;
        }
        public List<ResultCBIR> computeResultRFMethod(int iterationCount)
        {
            //assign iterationcount to a local variable for this class 
            numberOfIteration=iterationCount;
            //compute mean value and standard deviation od each image
            addFeatureValues();
            //Check min standard deviation 
            double minValue=checkMinStandardDeviation();
            //Add GaussianNormalizedWeight to each image bin/feature only for 1st iteration
            if (iterationCount == 0)
            {
                computeGaussianNormalizedWeight(minValue);
            }
            //Compute Minkowskidistance between each image 
            List <ResultCBIR> resultRFMethod = computeMinkowskiDistance();
            return resultRFMethod;
        }
        //Find normalized weight for each iteration
        public void computeNormalizedWeight(double sum)
        {
            foreach (RFMethodFeatureDetails bin in allFeature)
            {
                double normalizedWeight = bin.updatedWeight / sum;
                bin.normalizedWeight = normalizedWeight;
            }
        }

        //Retrieve RelevantImageDetails histogram from allImageHistogram RF method
        private void retireveRelevantImageHistogram(List<RelevantImage> relevantImages)
        {
            foreach (RelevantImage selectedImage in relevantImages)
            {
                selectedImage.ImageFound = false;
            }

            this.relevantImageHistogram.Clear();

            if (relevantImages.Count > 0)
            {
                  foreach (RFMethodImageDetails image in allImageDetails)
                 {
                     foreach (RelevantImage relevantImg in relevantImages)
                     {
                        if (relevantImg.ImageFound == false)
                        {
                            //compare filepath i.e. imageID 
                            if (new Uri(image.imageID) == new Uri(relevantImg.imageID))
                            {
                                RelevantImage userChoice = new RelevantImage();
                                relevantImg.ImageFound = true;
                                //Stroing the relevant image in an instance of RelevantImage class
                                userChoice.ImageFound = true;
                                userChoice.imageID = relevantImg.imageID;
                                userChoice.GaussianValue = image.GaussianNormalizedWeight;
                                //List<double> im = image.GaussianNormalizedWeight;
                                relevantImageHistogram.Add(userChoice);
                                //exit inner loop and goto next image in outter loop main histogram
                                break ;
                            }
                        }
                    }
                }
            }
        }
        //When RF method is clicked. When few relevant images are selected by user then call following function
        public List<ResultCBIR> computeNewFeatureMatrix(List<RelevantImage> selectedImages, List<RFMethodImageDetails> colorCodeIntensityGaussianValue)
        {
            allImageDetails = colorCodeIntensityGaussianValue;
            retireveRelevantImageHistogram(selectedImages);
            addFeatureValues();
            double minValue = checkMinStandardDeviation();
            computeUpdatedWeight(minValue);
            double sumUpdatedWeight=computeSumUpdatedWeight();
            computeNormalizedWeight(sumUpdatedWeight);
            List<ResultCBIR> resultRFMethod = computeMinkowskiDistance();
            return resultRFMethod;
        }
        public List<ResultCBIR> computeMinkowskiDistance()
        {
            //Clear result list before adding value
            this.DistancebetweenImage.Clear();
            List<double> selectedImage = new List<double>();
            foreach(RFMethodImageDetails eachImage in allImageDetails)
            {
                string image=eachImage.imageID;
                //if(image==queryImage)
                if (new Uri(image) == new Uri(queryImage))
                {
                    //compute distance between two images , 
                    selectedImage = eachImage.GaussianNormalizedWeight;
                    break;
                } 
            }

            foreach (RFMethodImageDetails histogramImage in allImageDetails)
            {
                ResultCBIR dis = new ResultCBIR();
                int binNumber = 0;
                double totalDistance = 0.0;
                while (binNumber < 89)
                {
                    double queryImage = selectedImage.ElementAt(binNumber);                   
                    double databaseImage = histogramImage.GaussianNormalizedWeight.ElementAt(binNumber);
                    RFMethodFeatureDetails featureSet =allFeature.ElementAt(binNumber);
                    double weight = featureSet.normalizedWeight;
                    double distance = weight * (Math.Abs(queryImage - databaseImage)); 
                    totalDistance = distance + totalDistance;
                    binNumber++;
                }
                dis.imageid = histogramImage.imageID;
                dis.totaldistance = totalDistance;
                DistancebetweenImage.Add(dis);
            }
            //Orderby 
            DistancebetweenImage = DistancebetweenImage.OrderBy(i => i.totaldistance).ToList();
            return DistancebetweenImage;
        }
    }
}
