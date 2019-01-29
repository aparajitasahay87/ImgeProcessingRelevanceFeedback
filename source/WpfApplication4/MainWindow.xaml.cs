using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Drawing;
using System.IO.IsolatedStorage;
namespace WpfApplication4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        List<double> imageIntensity = new List<double>();
        //New hashtable to store intensity of each image
        string selecteImageFilePath = null;       
        int pageLength = 20;
        int pageIndexMain = 0;      
        int maxPage = 0;
        int fileLength=0;
        List<string> files;
        List<double> eachImagePixelValue = new List<double>();
        List<HistogramBin> allimageDetailIntensityMethod = new List<HistogramBin>();
        List<string> intensityResultFiles = new List<string>();
        List<string> colorCodeResultFiles = new List<string>();
        List<string> rfMethodResultFiles = new List<string>();
        List<HistogramBinRFMethod> rfMethodHistogram = new List<HistogramBinRFMethod>();
        List<RelevantImage> relevantImage = new List<RelevantImage>();
        List<RFMethodImageDetails> colorCodeIntensityGaussianValue = new List<RFMethodImageDetails>();
        //Count number of iterations in RF method
        int rfMethodIterationCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            readDataBase();  
        }
        //Read the database.
        public void readDataBase()
        {
            string path = Environment.CurrentDirectory + "/" + "images";
            files = new List<string>(Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories));    
            fileLength = files.Count();
            maxPage = fileLength / pageLength;
            renderImage(files);
            //createImageHistogramBin(files);
            computePixelValue(files);
        }
        //Call RF method and pass histogram 
        public void createRFMethodDetails()
        {
            RfMethod rfMethodCall = new RfMethod();
            rfMethodCall.createHistogramRfMethod(allimageDetailIntensityMethod);
        }
        //Display image in UI
        public void renderImage(List<string> files)
        {
            try
            {
                int indexType = this.pageIndexMain;
                List<string> imagePerPage = computeImagePerPage(pageLength, indexType, files);
                //Add images of only one page 
                List<ImageDetails> imagelist = new List<ImageDetails>();
                foreach (string filename in imagePerPage)
                {
                    ImageDetails img = new ImageDetails();
                    img.imageName = filename;
                    if (RFMethod.IsChecked==true)
                    {
                        img.RfMethodSelected = "Visible";
                        img.otherMethodSelected = "Hidden";
                    }
                    else
                    {
                        img.otherMethodSelected = "Visible";
                        img.RfMethodSelected = "Hidden";
                    }
                    img.image = new Uri(filename);
                    img.IsChecked = relevantImage.Any(item => new Uri(item.imageID) == img.image);
                    imagelist.Add(img);
                }
                disp.ItemsSource = imagelist;
                pageNumber.Content = "Page" + (indexType + 1) + "of" + maxPage;
            }    
            catch (Exception e)
            {

            }
        }
        
        //Selected image to display
        private void imageselected_click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            //HistogramBin histogramBinIntensity = new HistogramBin();
            //histogramBinIntensity.initializeHistogramIntensityMethod();
            System.Windows.Controls.Image buttonImage = button.Content as System.Windows.Controls.Image; //Cntrl+alt+q (During debugging)
            if (buttonImage == null ||
               buttonImage.Source == null)
            {
                throw new Exception("bug!!");
            }
            string imagePath = buttonImage.Source.ToString();
            selecteImageFilePath = imagePath;
            //selectImage.Source = new Bitmap(imagePath,true);
            //new System.Windows.Media.Imaging.BitmapImage()
            selectImage.Source = new System.Windows.Media.Imaging.BitmapImage(
            new Uri(imagePath, UriKind.Absolute));

            if(this.IntensityMethod.IsChecked.GetValueOrDefault())
            {
                this.intensityMethod_click(null, null);
            }

            if( this.ColorCodeMethod.IsChecked.GetValueOrDefault())
            {
                this.colorCodeMethod_click(null, null);
            }
            if (this.RFMethod.IsChecked.GetValueOrDefault())
            {
                this.RFMethod_click(null, null);
            }
        }
        //Compute the pixel value and add it in histogram bin
        private void computePixelValue(List<string> files)
        {
            foreach (string filepath in files)
            {
                HistogramBin histogramBinIntensity = new HistogramBin();
                histogramBinIntensity.initializeHistogramIntensityMethod();
                histogramBinIntensity.initializehistogramColorCodeMethod();
                histogramBinIntensity.setImageId(filepath);

                Bitmap b = new Bitmap(filepath, true);
                int width =b.Width;
                int height = b.Height;
                int totalPixel = height * width;
                histogramBinIntensity.setNumberOfPixelImage(totalPixel);
                for(int i=0;i<width;i++)
                {
                    for(int j=0;j<height;j++)
                    {
                        Color pixel = b.GetPixel(i, j);
                        byte red =pixel.R;
                        byte green = pixel.G;
                        byte blue = pixel.B;

                        //Colorcode method
                        int colorCode = (blue & (192)) >> 6 | (green & (192)) >> 4 | (red & (192)) >> 2;
                        histogramBinIntensity.addElementHistogramColorCodeMethod(colorCode);
                        //Intensity method
                        double I = 0.299 * red + 0.587 * green + 0.114 * blue;
                        histogramBinIntensity.addIntensityToBin(I);
                    }
                }
                allimageDetailIntensityMethod.Add(histogramBinIntensity);
            }
            //call rf method and initialize histogram for RF method 
            RfMethod callRfMethod = new RfMethod();
            rfMethodHistogram = callRfMethod.createHistogramRfMethod(allimageDetailIntensityMethod);
        }
        //Click Intensity method radio button
        private void intensityMethod_click(object sender, RoutedEventArgs e)
        {
            string intensity = "intensity";
            this.RelevantImage.IsEnabled = false;
            this.setPageIndex(0);
            //pageIndexIntensity = 0;
            //pageIndexColorCode = 0;
            intensityResultFiles.Clear();
            ManhattanDistance distance = new ManhattanDistance();
            if(selecteImageFilePath==null)
            {
                MessageBox.Show("Please select one Image before we proceed");
                return;
            }
            List<ResultCBIR> sortedDistance = distance.computeManhattanDistance(selecteImageFilePath,
                    allimageDetailIntensityMethod,intensity);
            foreach (ResultCBIR fieldid in sortedDistance)
            {
                intensityResultFiles.Add(fieldid.imageid);
            }
            renderImage(intensityResultFiles);
        }
        //Click color code method button
        private void colorCodeMethod_click(object sender, RoutedEventArgs e)
        {
                string colorcode = "colorcode";
                this.RelevantImage.IsEnabled = false;
                setPageIndex(0);
                colorCodeResultFiles.Clear();
                ManhattanDistance distance = new ManhattanDistance();
                if (selecteImageFilePath == null)
                {
                    MessageBox.Show("Please select one Image before we proceed");
                    return;
                }
                List<ResultCBIR> sortedDistance = distance.computeManhattanDistance(selecteImageFilePath,
                        allimageDetailIntensityMethod, colorcode);
                foreach (ResultCBIR fieldid in sortedDistance)
                {
                    colorCodeResultFiles.Add(fieldid.imageid);
                }
                renderImage(colorCodeResultFiles);
        }
        //RF method click
        private void RFMethod_click(object sender, RoutedEventArgs e)
        {          
            setPageIndex(0);
            this.RelevantImage.IsEnabled = true;
            //Compute the rf method standard deviation, average value and minchowsky distance and return the result 
            //call compute RF method 
            rfMethodResultFiles.Clear();
            relevantImage.Clear();
            if (selecteImageFilePath == null)
            {
                MessageBox.Show("Please select one Image before we proceed");
                return;
            }

            RelevantImage queryImage = new RelevantImage();
            queryImage.imageID = selecteImageFilePath;
            relevantImage.Add(queryImage);

            RfMethod computeDistanceRfMethod = new RfMethod();
            computeDistanceRfMethod.setQueryImage(selecteImageFilePath);
            computeDistanceRfMethod.setMethodName("colorCodeIntensity");
            computeDistanceRfMethod.setallImageHistogramRfMethod(rfMethodHistogram);
            List<ResultCBIR> sortedDistance = computeDistanceRfMethod.computeResultRFMethod(0);//rfMethodIterationCount
            colorCodeIntensityGaussianValue = computeDistanceRfMethod.getGaussianValueImageDetails();
            //computeDistanceRfMethod.createFeatureMatrix();
            rfMethodIterationCount++;
            ////Add sorted imageid in a list 
            foreach (ResultCBIR fieldid in sortedDistance)
            {
                rfMethodResultFiles.Add(fieldid.imageid);
            }
            renderImage(rfMethodResultFiles);
        }
        private void RFRelevantImage_click(object sender,RoutedEventArgs e)
        {
            setPageIndex(0);
            rfMethodResultFiles.Clear();
            if(relevantImage.Count==0)
            {
                MessageBox.Show("Please select few relevant images before we proceed");
                return;
            }
            else
            {
                RfMethod Rf = new RfMethod();
                Rf.setMethodName("RF");
                Rf.setQueryImage(selecteImageFilePath);
                List<ResultCBIR> sortedDistance = Rf.computeNewFeatureMatrix(relevantImage, colorCodeIntensityGaussianValue);
                foreach (ResultCBIR fieldid in sortedDistance)
                {
                    rfMethodResultFiles.Add(fieldid.imageid);
                }
                renderImage(rfMethodResultFiles);
            }
        }

        private void relevantImage_click(object sender,RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            //HistogramBin histogramBinIntensity = new HistogramBin();
            //histogramBinIntensity.initializeHistogramIntensityMethod();
            System.Windows.Controls.Image checkBoxImage = checkBox.Content as System.Windows.Controls.Image; //Cntrl+alt+q (During debugging)
            if (checkBoxImage == null ||
               checkBoxImage.Source == null)
            {
                throw new Exception("bug!!");
            }
            string imagePath = checkBoxImage.Source.ToString();
            Uri currentImagePath = new Uri(imagePath);
            //Add queryImage in relevantImageFilePath
            if (checkBox.IsChecked == true)
            {
                RelevantImage relevantImages = new RelevantImage();
                relevantImages.imageID = imagePath;
                relevantImage.Add(relevantImages);
            }
            else
            {
                RelevantImage existingImage = relevantImage.FirstOrDefault(item => new Uri(item.imageID) == currentImagePath);
                relevantImage.Remove(existingImage);
            }
        }
        /*Code below Display the images in pages */
        public List<string> computeImagePerPage(int pageLength, int pageIndex, List<string> files)
        {
            List<string> ImagePerPage = new List<string>();
            List<string> EmptyImagePerPage = new List<string>();
            int startIndex = 0;
            int endIndex = 0;
            if (startIndex >= 0 && endIndex < fileLength)
            {
                startIndex = pageIndex * pageLength;
                endIndex = startIndex + pageLength - 1;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    ImagePerPage.Add(files.ElementAt(i));
                }
                return ImagePerPage;
            }
            else
            {
                return EmptyImagePerPage;
            }
        }


        //Method the detect which file to display 
        public List<string> cbirMethodType()
        {
            if (IntensityMethod.IsChecked == true)
            {
                return intensityResultFiles;
            }
            else if (ColorCodeMethod.IsChecked == true)
            {
                return colorCodeResultFiles;
            }
            else
            {
                return files;
            }
        }

        //Previous page button click event
        private void previousPage_Click(object sender, RoutedEventArgs e)
        {
            Button buttonPrev = sender as Button;
            int pageIndex = this.pageIndexMain;
            this.setPageIndex(--pageIndex);
            List<string> files = cbirMethodType();
            renderImage(files);
        }

        //Check image checkbox value selected or not 
        private void checkClick(object sender , RoutedEventArgs e)
        {
            this.IsEnabled = true;
        }
        public void setPageIndex(int index)
        {
            if (index < 0 || index >= this.maxPage)
            {
                return;
            }

            this.pageIndexMain = index;

            if (this.pageIndexMain == 0)
            {
                this.Previouspage.IsEnabled = false;
            }
            else
            {
                this.Previouspage.IsEnabled = true;
            }

            if (this.pageIndexMain == this.maxPage - 1)
            {
                this.Nextpage.IsEnabled = false;
            }
            else
            {
                this.Nextpage.IsEnabled = true;
            }
        }

        //Next page butoon click event
        private void nextPage_click(object sender, RoutedEventArgs e)
        {   
            Button buttonNext = sender as Button;
            int pageIndex = this.pageIndexMain;
            this.setPageIndex(++pageIndex);
            List<string> files = cbirMethodType();
            renderImage(files);
           
        }

        public bool checkIntermediateStateOfPage(int pageIndex)
        {
            if(pageIndex>0 && pageIndex<maxPage)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
