using LuisTalk.Utilities;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Windows.Storage.Streams;
using System.Reflection;

namespace LuisTalk.Models.Services
{
    public class EmotionApi
    {
        private EmotionServiceClient _client;

        public EmotionApi(string subscriptionKey)
        {
            Check.Required<ArgumentNullException>(() => subscriptionKey != null);

            _client = new EmotionServiceClient(subscriptionKey);
        }

        public async Task<string> GetPrevalentEmotion(IRandomAccessStream imageStream)
        {
            var result = await _client.RecognizeAsync(imageStream.AsStreamForRead());

            var emotionScores = typeof(Microsoft.ProjectOxford.Emotion.Contract.Scores).GetProperties()
                .Where(p => p.PropertyType == typeof(float))
                .Select(p => new { Name = p.Name, Score = (float)p.GetValue(result[0].Scores) });
                

            return emotionScores.OrderByDescending(s => s.Score).Select(s => s.Name).FirstOrDefault();
        }
    }
}
