using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Windows;
using Windows.Media;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Emotion;
using System.Diagnostics;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{

    public class DataItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public DataItem(string n, int v)
        {
            Name = n;
            Value = v;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FaceDetector faceDetector;
        IList<DetectedFace> detectedFaces;

        EmotionServiceClient emoClient = new EmotionServiceClient("74cc34868edc4ad5a76d637e2fb5caa4");
        IFaceServiceClient faceServiceClient = new FaceServiceClient("86a9c1bbf12f4b7890602265d61b8b18");

        public ObservableCollection<DataItem> Items { get; set; } = new ObservableCollection<DataItem>();

        private readonly SolidColorBrush lineBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly double lineThickness = 2.0;
        private readonly SolidColorBrush fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);

        public string personGroupId;

        public MainPage()
        {
            this.InitializeComponent();
            Graph.DataContext = this;            
        }        

        //Start the process
        private async void button_Click(object sender, RoutedEventArgs e)
        {

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");
            folderPicker.FileTypeFilter.Add(".bmp");
            folderPicker.ViewMode = PickerViewMode.Thumbnail;

            StorageFolder photoFolder = await folderPicker.PickSingleFolderAsync();
            if (photoFolder == null)
            {
                return;
            }

            var files = await photoFolder.GetFilesAsync();

            List<Scores> E = new List<Scores>();
            int[] num = new int[files.Count];            

            for (int i = 0; i<files.Count; i++)
            {   

                IRandomAccessStream fileStream = await files[i].OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                BitmapTransform transform = new BitmapTransform();
                const float sourceImageHeightLimit = 1280;

                if (decoder.PixelHeight > sourceImageHeightLimit)
                {
                    float scalingFactor = (float)sourceImageHeightLimit / (float)decoder.PixelHeight;
                    transform.ScaledWidth = (uint)Math.Floor(decoder.PixelWidth * scalingFactor);
                    transform.ScaledHeight = (uint)Math.Floor(decoder.PixelHeight * scalingFactor);
                }

                SoftwareBitmap sourceBitmap = await decoder.GetSoftwareBitmapAsync(decoder.BitmapPixelFormat, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                const BitmapPixelFormat faceDetectionPixelFormat = BitmapPixelFormat.Gray8;

                SoftwareBitmap convertedBitmap;

                if (sourceBitmap.BitmapPixelFormat != faceDetectionPixelFormat)
                {
                    convertedBitmap = SoftwareBitmap.Convert(sourceBitmap, faceDetectionPixelFormat);
                }
                else
                {
                    convertedBitmap = sourceBitmap;
                }

                if (faceDetector == null)
                {
                    faceDetector = await FaceDetector.CreateAsync();
                }
                               
                detectedFaces = await faceDetector.DetectFacesAsync(convertedBitmap);

                Scores sc = null;

                if (detectedFaces.Count > 0)
                {
                    num[i] = detectedFaces.Count;
                    FaceRectangle rectID = new FaceRectangle();
                    rectID = await UploadAndDetectFaces(files[i].Path);
                    if (rectID != null)
                    {
                        sc=await EstimateEmotions(files[i].Path, rectID);
                    }
                                        
                }

                E.Add(sc);
                if (sc!=null) Items.Add(new DataItem(i.ToString(), (int)(sc.Happiness * 100)));

                sourceBitmap.Dispose();
                fileStream.Dispose();
                convertedBitmap.Dispose();
            }
              
        }

        private async Task<Scores> EstimateEmotions(String imageFilePath, FaceRectangle rectID)
        {            
            try
            {
                StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(imageFilePath));
                StorageFile sampleFile = await storageFolder.GetFileAsync(System.IO.Path.GetFileName(imageFilePath));
                var inputStream = await sampleFile.OpenReadAsync();
                Stream stream = inputStream.AsStreamForRead();               

                var emo_rec = await emoClient.RecognizeAsync(stream);

                if (emo_rec != null && emo_rec.Length>0)
                {
                  
                    List<Microsoft.ProjectOxford.Common.Rectangle> emoRects = new List<Microsoft.ProjectOxford.Common.Rectangle>();                    
                    
                    for (int p = 0; p < emo_rec.Length; p++)
                    {
                        emoRects.Add(emo_rec[p].FaceRectangle);
                    }

                    int matchIndex = GetMatchingIndex(rectID, emoRects);
                    return emo_rec[matchIndex].Scores;
                }
                else
                {                
                                       
                }                            
                  
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException, e.StackTrace);
            }
            return null;
        }

        private async Task<FaceRectangle> UploadAndDetectFaces(String imageFilePath)
        {
            
            try
            {
                Windows.Storage.StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(imageFilePath));
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(System.IO.Path.GetFileName(imageFilePath));
                var inputStream = await sampleFile.OpenReadAsync();
                Stream stream = inputStream.AsStreamForRead();

                var faces = await faceServiceClient.DetectAsync(stream);
                var faceRects = faces.Select(face => face.FaceRectangle);                         
                                
                var faceIds = faces.Select(face => face.FaceId).ToArray();
                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                for (int j = 0; j<results.Length; j++)
                {
                    Debug.WriteLine("Result of face: {0}", results[j].FaceId);
                    if (results[j].Candidates.Length == 0)
                    {
                        Debug.WriteLine("No one identified");
                    }
                    else
                    {                       
                        var candidateId = results[j].Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        Debug.WriteLine("Identified as {0}", person.Name);

                        if (person.Name == "person1")
                        {
                            FaceRectangle rectID;
                            return rectID = faces[j].FaceRectangle;
                        }
                    }
                }                

            }
            catch (Microsoft.ProjectOxford.Face.FaceAPIException exc)
            {
                Debug.WriteLine(exc.ErrorCode);                
            }
            return null;
        }


        private int RectDist(FaceRectangle rectID, Microsoft.ProjectOxford.Common.Rectangle rectE)
        {
            int xcID = Convert.ToInt32(Math.Round(Convert.ToDecimal(rectID.Left + rectID.Width/2)));
            int ycID = Convert.ToInt32(Math.Round(Convert.ToDecimal(rectID.Top + rectID.Height / 2)));
            int xcE = Convert.ToInt32(Math.Round(Convert.ToDecimal(rectE.Left + rectE.Width / 2)));
            int ycE = Convert.ToInt32(Math.Round(Convert.ToDecimal(rectE.Top + rectE.Height / 2)));            

            int dx = xcID - xcE;
            int dy = ycID - ycE;

            int dist = dx^2 + dy^2;

            return dist;
        }

        private int GetMatchingIndex(Microsoft.ProjectOxford.Face.Contract.FaceRectangle rectID, List<Microsoft.ProjectOxford.Common.Rectangle> rectsE)
        {
            int[] D = new int[rectsE.Count()];
            for(int k = 0; k<D.Length; k++)
            {
                D[k] = RectDist(rectID, rectsE[k]);
            }

            int minD = D.Min();
            int minI = D.ToList().IndexOf(minD);

            return minI;
        }
         
        private async void button1_Click(object sender, RoutedEventArgs e)
        {           
         
            personGroupId = "1";
            try
            {
                await faceServiceClient.DeletePersonGroupAsync(personGroupId);
            }
            catch (Microsoft.ProjectOxford.Face.FaceAPIException exc)
            {
                Debug.WriteLine(exc.ErrorCode);
            }

            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, "PG");
            }
            catch (Microsoft.ProjectOxford.Face.FaceAPIException exc)
            {
                Debug.WriteLine(exc.ErrorCode);
            }

            String[] listOfPersons = { "person1" };
            CreatePersonResult[] person = new CreatePersonResult[listOfPersons.Length];
            string[] personImageDir = new string[listOfPersons.Length];

            for (int i = 0; i < listOfPersons.Length; i++)
            {

                person[i] = await faceServiceClient.CreatePersonAsync(personGroupId, listOfPersons[i]);

                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add(".jpg");
                folderPicker.FileTypeFilter.Add(".jpeg");
                folderPicker.FileTypeFilter.Add(".png");
                folderPicker.FileTypeFilter.Add(".bmp");
                folderPicker.ViewMode = PickerViewMode.Thumbnail;

                StorageFolder photoFolder = await folderPicker.PickSingleFolderAsync();
                if (photoFolder == null)
                {
                    return;
                }

                var files = await photoFolder.GetFilesAsync();

                foreach (var file in files)
                {
                    var inputStream = await file.OpenReadAsync();
                    Stream stream = inputStream.AsStreamForRead();
                    await faceServiceClient.AddPersonFaceAsync(personGroupId, person[i].PersonId, stream);                    
                }
            }

            await faceServiceClient.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (!trainingStatus.Status.Equals("running"))
                {
                    break;
                }

                await Task.Delay(1000);
            }
            
        }
    }
}