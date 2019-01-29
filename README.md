# ImgeProcessingRelevanceFeedback
c# xaml project that uses relevance feddback method tp compare query image with database image
Multimedia Database: Assignment 2 
Aparajita Sahay
CBIR Method: Relevance Feedback
Reading:
In relevance feedback approach both taught in class and mentioned in reference paper[RUI98], both of the image retrieval process is interactive between computer and human. The relevance feedback tries to capture the difference between high level concepts and low level feature from user’s feedback.
1)	Both the methods takes user feedback to find relevant images. In both the cases user don’t have to mention the detailed or intricate features of the image. A high level relevant of an image is accepted by users. Then the algorithm accordingly updates the feature matrix and show the relevant images. 
2)	Both, initialize the weights to no bias weights. That is, every entity is initially of same importance. And as the user choose the relevant images, then the weights get updated and used for further computing similar images.
3)	For normalization we have used Gaussian Normalized weight. In paper different ways for normalization is provided such as Intra normalization and inter normalization. Gaussian normalization is Intra-Normalization where procedure ensures equal emphasis of each component within a representation vector. However, in inter normalization procedure equal emphasis of each indivisual similarity value within overall similarity. Paper uses inter normalization procedure to compute CBIR.
Following step is different from our approach where they use inter normalization procedure:-
1.	Using inter normalization procedure, we compute similarity between two images.



2. There can be  possible similarity between any images. Consider it as data sequence find mean and standard deviation of the sequence.
3. First compute un-normalized similarity between query image and database image:
 
4. Then, compute normalized similarity using : 
 
Then after the above mentioned step to compute weight both the procedure taught in class and mentioned in paper uses normalized weight/total weight to computed weight.




Report
1.	Coding Environment used: Visual Studio Community 2013. I have developed a Windows Presentation Foundation (WPF) application. 
2.	Technology used: c# and xaml.
3.	Steps to run the program: Create a “images” folder at the same location where WpfApplication4.exe is located or saved. “images” folder consist 100 images provided as the sample image.
4.	Step by step method to use the system:
•	WpfApplication4.exe will take around 1-2 minutes to load. Because every time the program is loaded ,each image pixel value is computed and is kept inside the histogram-bins.
•	Once program is loaded, a user interface is displayed. It will show list of images. Each page will show 20 images. 
•	System will have radio buttons for Intensity method ,color-code method, color code&Intensity, Relevence Feedback method and page navigation button next and previous.
•	When the program loads, relevance feedback button will not be enabled because first we have to click on “colorCodeAndIntensity” method option to get first set of result with no bias weights for every feature. Once “colorCodeAndIntensity” method is clicked then “RelevanceFeedback” method will be enabled to compute further results. 
•	Once “colorCodeAndIntensity” method is clicked then user can select few relevant images. By default I have selected the query image as a relevant image in the result images, in addition to that user can select as many relevant images he/she feels like close to query image to compute further relevant image results.
•	Select an image then choose radio button which method you want to see, once the method is selected, result will be shown.
•	If user selects a radio button or method type without selecting the image, a message box will be displayed to select image before moving ahead.
•	Once any radio button is selected, relevant images are shown from page 1 to page 5. Each page has 20 images. Page navigation can be done using next and previous button. Next and previous button will be automatically enabled and disabled depending upon the available images. Read image from left to right. 
•	Note: For relevance feedback image search results are in same order for query image 1 but there is one image in the list that get swapped with it’s neighboring element. I discussed the problem with few of my class mates who were developing the system using c# and they faced the same issue. 
•	4 screenshots for query image 1 is added below.
5.	Precision Value with image 1 as query image :
a.	1st iteration: = 8/20 , out of 20 retrieved image 8 were relevant images
b.	2nd iteration = 12/20.
c.	3rd iteration : 14/20
As the user add more relevant feedback the precision value of a image is increasing.

![relevanceffedback](https://github.com/aparajitasahay87/ImgeProcessingRelevanceFeedback/blob/master/relevanceFeedback.png)




 



 

 



 
