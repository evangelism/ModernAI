using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
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
using Evangelism.ProjectOxford;
using System.Net.Http;
using System.Collections.ObjectModel;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmotionAnalysis.Video
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
        public ObservableCollection<DataItem> Items { get; set; } = new ObservableCollection<DataItem>();
        dynamic result;
        public MainPage()
        {
            this.InitializeComponent();
            Graph.DataContext = this;
            foreach (var x in new string[] { "happiness", "anger", "contempt","disgust","surprise","fear","neutral","sadness" })
                Combo.Items.Add(x);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var f = File.ReadAllText("sample-frame.json");
            result = Newtonsoft.Json.JsonConvert.DeserializeObject(f);
            Draw(result, 0, "happiness");
        }

        GenericClient Cli = new GenericClient("0b813cba8a5c4b7bbd52dabd1263853d");

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            log.Text = "Analyzing...";

            string opurl = await SubmitVideo(url.Text);
            while (true)
            {
                dynamic r = await Cli.GetDynamic(opurl);
                log.Text = r.status;
                if (r.status == "Succeeded")
                {
                    result = r.processingResult;
                    break;
                }
                if (r.status == "Running")
                {
                    var f = float.Parse(r.progress.ToString());
                    pro.Value = f;
                }
                if (r.status == "Failed")
                    break;
                await Task.Delay(30000);
            }
            if (result == null) return;
            Draw(result,0);
        }

        void Draw(dynamic result,int id, string emo = "happiness")
        {
            int n = 0;
            foreach(dynamic frag in result.fragments)
            {
                if (frag["events"]!=null)
                {
                    foreach(var fr in frag.events)
                    {
                        foreach (var p in fr)
                        {
                            if (p.id==id)
                            {
                                Items.Add(new DataItem(n++.ToString(), (int)(100*(float)p.scores[emo])));          
                            }
                        }
                    }
                }
            }
        }

        // Submit video for analysis
        // Returns the URL that can be used to retrieve status/result
        private async Task<string> SubmitVideo(string url)
        {
            HttpResponseMessage rec = await Cli.PostResponseAsync("https://api.projectoxford.ai/emotion/v1.0/recognizeinvideo?outputStyle=perFrame", "{\"url\":\"" + url + "\"}");
            log.Text += "Done";
            string opurl = null;
            foreach (var k in rec.Headers)
            {
                if (k.Key == "Operation-Location") opurl = k.Value.First();
            }

            return opurl;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Items.Clear();
            Draw(result, 0, (string)Combo.SelectedItem);
        }
    }
}
