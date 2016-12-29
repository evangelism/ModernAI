using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HappyFace
{

    public class ImgData
    {
        public string Url { get; set; }
        public BitmapImage Img { get; set; }
        public int Value { get; set; }
    }

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public string APIKey = "32217dc77dab4497952b046f3398da06";
        public string APIKeyOx = "c89be8532536452d996130a6083d3ca5";

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += StartWork;
        }

        private async void StartWork(object sender, RoutedEventArgs e)
        {
            var Ox = new EmotionServiceClient(APIKeyOx);
            var l = await Search("bill+gates");
            gv.ItemsSource = l;
            float max = 0;
            foreach(var x in l)
            {
                var res = await Ox.RecognizeAsync(x.Url);
                if (res.Length>0)
                {
                    var n = res[0].Scores.Anger;
                    x.Value = (int)(n*1000);
                    if (n>max)
                    {
                        max=n;
                        img.Source = x.Img;
                    }
                }
                await Task.Delay(300);
            }
            gv.ItemsSource = null;
            gv.ItemsSource = l;
        }

        private async Task<List<ImgData>> Search(string q)
        {
            var client = new HttpClient();     
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", APIKey);
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?count=20&q=" + q;
            var response = await client.GetStringAsync(uri);
            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            var L = new List<ImgData>();
            foreach (var z in x.value)
            {
                var u = z.contentUrl.ToString();
                var bi = new BitmapImage(new Uri(u));
                L.Add(new ImgData() { Img = bi, Url = u, Value=0 });
            }
            return L;
        }
    }
}
