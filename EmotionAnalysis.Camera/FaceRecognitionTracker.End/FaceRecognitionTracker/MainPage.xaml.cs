using Microsoft.ProjectOxford.Emotion;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Это приложение получает ваше изображение с веб-камеры и
// распознаёт эмоции на нём, обращаясь к Cognitive Services
// Предварительно с помощью Windows UWP API анализируется, есть
// ли на фотографии лицо.

// Эмоции затем сериализуются в формат JSON. Они становятся доступны
// в строке, помеченной TODO:

namespace FaceRecognitionTracker
{
    public sealed partial class MainPage : Page
    {

        #region Variables
        MediaCapture MC;
        DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };
        EmotionServiceClient Oxford = new EmotionServiceClient(Config.OxfordAPIKey);

        FaceDetectionEffect FaceDetector;
        VideoEncodingProperties VideoProps;

        bool IsFacePresent = false;

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Первоначальная инициализация страницы
        /// </summary>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Init();
            dt.Tick += GetEmotions;
            dt.Start();
        }

        /// <summary>
        /// Инициализирует работу с камерой и с локальным распознавателем лиц
        /// </summary>
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
            def.DetectionMode = FaceDetectionMode.HighPerformance;
            FaceDetector = (FaceDetectionEffect)(await MC.AddVideoEffectAsync(def, MediaStreamType.VideoPreview));
            FaceDetector.FaceDetected += FaceDetectedEvent;
            FaceDetector.DesiredDetectionInterval = TimeSpan.FromMilliseconds(100);
            FaceDetector.Enabled = true;

            await MC.StartPreviewAsync();
            var props = MC.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            VideoProps = props as VideoEncodingProperties;
        }

        /// <summary>
        /// Срабатывает при локальном обнаружении лица на фотографии.
        /// Рисует рамку и устанавливает переменную IsFacePresent=true
        /// </summary>
        private async void FaceDetectedEvent(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HighlightDetectedFace(args.ResultFrame.DetectedFaces.FirstOrDefault()));
        }

        /// <summary>
        /// Отвечает за рисование прямоугольника вокруг лица
        /// </summary>
        /// <param name="face">Обнаруженное лицо</param>
        /// <returns></returns>
        private async Task HighlightDetectedFace(DetectedFace face)
        {
            var cx = ViewFinder.ActualWidth / VideoProps.Width;
            var cy = ViewFinder.ActualHeight / VideoProps.Height;

            if (face == null)
            {
                FaceRect.Visibility = Visibility.Collapsed;
                Info.Visibility = Visibility.Collapsed;
                IsFacePresent = false;
            }
            else
            {
                Info.Margin = FaceRect.Margin = new Thickness(cx * face.FaceBox.X, cy * face.FaceBox.Y, 0, 0);
                Info.Width = FaceRect.Width = cx * face.FaceBox.Width;
                Info.Height = FaceRect.Height = cy * face.FaceBox.Height;
                FaceRect.Visibility = Visibility.Visible;
                Info.Visibility = Visibility.Visible;
                IsFacePresent = true;
            }
        }

        /// <summary>
        /// Основная функция, срабатывающая по таймеру
        /// Распознаёт эмоции
        /// </summary>
        async void GetEmotions(object sender, object e)
        {
            if (!IsFacePresent) return;
            dt.Stop(); 
            var ms = new MemoryStream();
            try
            {
                // Запоминаем фотографию в поток в памяти
                await MC.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), ms.AsRandomAccessStream());
            }
            catch { dt.Start(); return; }

            ms.Position = 0L;
            var Emo = await Oxford.RecognizeAsync(ms); 
            // ^^^ основной вызов распознавателя эмоций
            if (Emo != null && Emo.Length > 0)
            // если обнаружено одно и более лицо
            {
                var Face = Emo[0]; // берем первое (нулевое) лицо
                                   // Face.Scores - запись с различными эмоциями (Fear,Surprise,...)

                Info.Text = ((int)(100*Face.Scores.Happiness)).ToString();

            }
            dt.Start(); // перезапускаем таймер
        }
    }
}

