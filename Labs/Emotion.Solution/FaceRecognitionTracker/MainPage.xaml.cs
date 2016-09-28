using Microsoft.ProjectOxford.Emotion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FaceRecognitionTracker
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaCapture MC;

        FaceDetectionEffect FaceDetector;
        VideoEncodingProperties VideoProps;

        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };

        bool IsFacePresent = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        EmotionServiceClient Oxford;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Init();
            dt.Tick += GetEmotions;
            dt.Start();
            Oxford = new EmotionServiceClient("0b813cba8a5c4b7bbd52dabd1263853d");
        }

        async void GetEmotions(object sender, object e)
        {
            if (!IsFacePresent) return;
            dt.Stop();
            var ms = new MemoryStream();
            try
            {
                await MC.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), ms.AsRandomAccessStream());
            }
            catch
            {
                dt.Start();
                return;
            }
            ms.Position = 0L;
            var Emo = await Oxford.RecognizeAsync(ms);
            if (Emo != null && Emo.Length > 0)
            {
                var Face = Emo[0];
                System.Diagnostics.Debug.WriteLine(Face.Scores.Happiness);
                txt.Text = Face.Scores.Happiness.ToString();
            }
            dt.Start();
        }

        private async Task Init()
        {
            MC = new MediaCapture();
            var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var camera = cameras.First();
            var settings = new MediaCaptureInitializationSettings() { VideoDeviceId = camera.Id };
            await MC.InitializeAsync(settings);
            ViewFinder.Source = MC;

            // Create face detection
            var def = new FaceDetectionEffectDefinition();
            def.SynchronousDetectionEnabled = false;
            def.DetectionMode = FaceDetectionMode.HighQuality;
            FaceDetector = (FaceDetectionEffect)(await MC.AddVideoEffectAsync(def, MediaStreamType.VideoPreview));
            FaceDetector.FaceDetected += FaceDetectedEvent;
            FaceDetector.DesiredDetectionInterval = TimeSpan.FromMilliseconds(100);
            FaceDetector.Enabled = true;

            await MC.StartPreviewAsync();
            var props = MC.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            VideoProps = props as VideoEncodingProperties;
        }

        private async void FaceDetectedEvent(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HighlightDetectedFace(args.ResultFrame.DetectedFaces.FirstOrDefault()));
        }

        private async Task HighlightDetectedFace(DetectedFace face)
        {
            var cx = ViewFinder.ActualWidth / VideoProps.Width;
            var cy = ViewFinder.ActualHeight / VideoProps.Height;

            if (face == null)
            {
                FaceRect.Visibility = Visibility.Collapsed;
                IsFacePresent = false;
            }
            else
            {
                FaceRect.Margin = new Thickness(cx * face.FaceBox.X, cy * face.FaceBox.Y, 0, 0);
                FaceRect.Width = cx * face.FaceBox.Width;
                FaceRect.Height = cy * face.FaceBox.Height;
                FaceRect.Visibility = Visibility.Visible;
                IsFacePresent = true;
            }
        }

    }
}

