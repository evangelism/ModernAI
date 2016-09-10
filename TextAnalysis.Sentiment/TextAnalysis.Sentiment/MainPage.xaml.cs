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
                        var R = await Client.AnalyzeSentiment(Store);
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

        public async Task Summarize()
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
                    if (Store.documents.Count > 0)
                    {
                        var R = await Client.ExtractKeyphrases(Store);
                        StringBuilder z = new StringBuilder();
                        z.AppendLine($"CHAPTER {c}");
                        foreach (var d in R.documents)
                        {
                            z.AppendLine(string.Join(",", d.keyPhrases));
                        }
                        Summary.Text += z.ToString();
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
                        if (Store.documents.Count>0 && Store.documents.Last().text.Length+sb.ToString().Length<5000)
                        {
                            System.Diagnostics.Debug.WriteLine($"Last {Store.documents.Last().text.Length}, sb {sb.Length}");
                            Store.documents.Last().text += "\n\r" + sb.ToString();
                        }
                        else Store.documents.Add(new TextAnalysisDocument(key, "en", sb.ToString()));
                    }
                    sb.Clear();
                    p++;
                    continue;
                }
                sb.AppendLine(s);
            }
        }


        private async void AnalyzeClick(object sender, RoutedEventArgs e)
        {
            Summary.Visibility = Visibility.Collapsed;
            PosGrid.Visibility = Visibility.Visible;
            Graph.Visibility = Visibility.Visible;
            await Analyze();
        }

        private async void SummaryClick(object sender, RoutedEventArgs e)
        {
            Summary.Visibility = Visibility.Visible;
            PosGrid.Visibility = Visibility.Collapsed;
            Graph.Visibility = Visibility.Collapsed;
            await Summarize();
        }
    }
}
