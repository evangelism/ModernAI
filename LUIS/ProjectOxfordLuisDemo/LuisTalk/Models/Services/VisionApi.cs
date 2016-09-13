using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using LuisTalk.Utilities;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;
using System.Reflection;

namespace LuisTalk.Models.Services
{
    public class VisionApi
    {
        private IVisionServiceClient _client;
        public VisionApi(string subscriptionKey)
        {
            Check.Required<ArgumentNullException>(() => subscriptionKey != null);

            _client = new VisionServiceClient(subscriptionKey);
        }

        public async Task<IRandomAccessStream> GetThumbnailImage(IRandomAccessStream stream, int width, int height, bool smartCropping = true)
        {
            var response = await _client.GetThumbnailAsync(stream.AsStreamForRead(), width, height, smartCropping);

            var responseStream = new MemoryStream(response);
            return responseStream.AsRandomAccessStream();
        }

        public async Task<string> RecognizeTextAsync(IRandomAccessStream stream, string language = "unk", bool detectOrientation = true)
        {
            var response = await _client.RecognizeTextAsync(stream.AsStreamForRead(), language, detectOrientation);

            return response.Regions.SelectMany(r => r.Lines).SelectMany(l => l.Words).Select(w => w.Text).StringJoin(" ");
        }
    }
}
