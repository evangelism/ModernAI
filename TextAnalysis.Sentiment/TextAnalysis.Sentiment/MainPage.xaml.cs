using Evangelism;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextAnalysis.Sentiment
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
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random Rnd = new Random();

        public ObservableCollection<DataItem> Items { get; set; } 
            = new ObservableCollection<DataItem>();

        public MainPage()
        {
            this.InitializeComponent();
            Data.DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Analyze();
        }

        public async Task Analyze()
        {
            TextAnalysisClient Client = new TextAnalysisClient(Config.TextAnalysisApiKey);

            string fname = @"Data\wap.txt";
            StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await InstallationFolder.GetFileAsync(fname);
            var stream = await file.OpenReadAsync();
            var sr = new StreamReader(stream.AsStreamForRead());
            int b = 0; // BOOK
            int c = 0; // Chapter
            int p = 0; // Paragraph

            double mp = 0; // most positive score
            double mn = 1; // most negative score

            StringBuilder sb = new StringBuilder();
            TextAnalysisDocumentStore Store = new TextAnalysisDocumentStore();

            while (!sr.EndOfStream)
            {
                var s = await sr.ReadLineAsync();
                if (s.Contains("BOOK"))
                {
                    b++;
                    c = 0;
                    if (b > 2) break;
                    continue;
                }
                if (s.Contains("CHAPTER"))
                {
                    if (sb.Length > 20)
                    {
                        var key = $"b{b}c{c}p{p}";
                        Store.documents.Add(new TextAnalysisDocument(key, "en", sb.ToString()));
                    }
                    sb.Clear();
                    if (Store.documents.Count > 2)
                    {
                        await Task.Delay(3000); // Pause to make sure service is not called to frequently
                        var R = await Client.Analyze(Store);
                        var r = R.documents.Count==0 ? 0 :
                            (from x in R.documents
                                 select x.score).Average();
                        Items.Add(new DataItem($"b{b}c{c}", (int)(r * 100)));
                        foreach (var x in R.documents)
                        {
                            if (x.score >= mp)
                            {
                                mp = x.score;
                                pos.Text = x.text;
                                posh.Text = $"Positive score={mp}";
                            }
                            if (x.score <= mn)
                            {
                                mn = x.score;
                                neg.Text = x.text;
                                negh.Text = $"Negative score={mn}";
                            }
                        }
                    }
                    Store.documents.Clear();
                    c++;
                    p = 0;
                    continue;
                }
                if (s.Trim().Equals(string.Empty))
                {
                    if (sb.Length > 20)
                    {
                        var key = $"b{b}c{c}p{p}";
                        Store.documents.Add(new TextAnalysisDocument(key, "en", sb.ToString()));
                    }
                    sb.Clear();
                    p++;
                    continue;
                }
                sb.AppendLine(s);
            }
        }
    }
}
