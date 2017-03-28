using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Common.Contract;
using System.Threading.Tasks;

namespace FindFaceBot
{
    public class FaceFinder
    {
        public async Task<string> Find(string name, string emo)
        {
            var S = new BingImageSearch(Config.BingKey);
            var E = new EmotionServiceClient(Config.EmotionKey);
            var res = await S.Search(name, 7);
            float m = -1, t;
            string u = "";
            foreach (var x in res)
            {
                var r = await E.RecognizeAsync(x);
                if (r.Length>0 && (t=eget(r[0].Scores,emo))>m)
                {
                    m = t; u = x;
                }
            }
            return u;
        }

        private float eget(EmotionScores scores, string emo)
        {
            if (emo == "Happy") return scores.Happiness;
            if (emo == "Sad") return scores.Sadness;
            if (emo == "Angry") return scores.Anger;
            if (emo == "Surprise") return scores.Surprise;
            return 0;
        }
    }
}