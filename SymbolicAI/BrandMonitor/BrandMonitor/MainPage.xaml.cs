using Evangelism.ConceptGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BrandMonitor
{

    public struct BrandInfo
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int Width => Count * 50;
    }

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            lb.DataContext = this;
            Tweetinvi.Auth.SetUserCredentials(Config.Twi_Consumer_Key, Config.Twi_Consumer_Secret, Config.Twi_User_Access_Key, Config.Twi_Access_Secret);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var str = Tweetinvi.Stream.CreateSampleStream();
            str.AddTweetLanguageFilter(LanguageFilter.English);
            str.TweetReceived += ProcessTweet;
            await str.StartStreamAsync();
        }

        public ObservableCollection<BrandInfo> BrandData { get; set; } = new ObservableCollection<BrandInfo>();

        ConceptGraphCachingClient CG = new ConceptGraphCachingClient(Config.CG_API_Key);

        private async void ProcessTweet(object sender, TweetReceivedEventArgs e)
        {
            var t = e.Tweet;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                log.Text = $"{t.Text}";
                await ProcessTweet(t.Text);
            });
        }

        private async Task ProcessTweet(string txt)
        {
            foreach (var x in txt.Split(' ', ',', '.', ':', '!', '(', ')'))
            {
                if (x.Length > 4 && !x.StartsWith("#") && !x.StartsWith("@"))
                {
                    var res = await CG.QueryProb(x, 3);
                    if (res == null) continue;
                    if (res.Select(z => z.Name).Contains("brand"))
                    {
                        var done = false;
                        for (int i = 0; i < BrandData.Count; i++)
                        {
                            if (BrandData[i].Name == x.ToLower())
                            {
                                BrandData[i] = new BrandInfo() { Name = BrandData[i].Name, Count = BrandData[i].Count + 1 };
                                done = true;
                            }
                        }
                        if (!done) BrandData.Add(new BrandInfo() { Name = x.ToLower(), Count = 1 });
                    }
                }
            }

        }

    }
}
