using LuisTalk.Models.Services;
using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace LuisTalk.Models.ConcreteDialogs
{
    public class HowAreYouDialog: Dialog
    {
        private SentimentAnalysisService _sentimentAnalysisService;
        private EmotionApi _emotionApi;

        public HowAreYouDialog(SentimentAnalysisService sentimentAnalysisService, EmotionApi emotionApi)
            :base("HowAreYou")
        {
            Check.Required<ArgumentNullException>(() => sentimentAnalysisService != null);
            Check.Required<ArgumentNullException>(() => emotionApi != null);

            _sentimentAnalysisService = sentimentAnalysisService;
            _emotionApi = emotionApi;
        }

        public override string Initialize()
        {
            CurrentStep = new DialogStep(ProcessResponseOnHowAreYouText, ProcessResponseOnHowAreYouImage);

            return "Hey! How are you?";
        }

        private async Task<DialogStepResult> ProcessResponseOnHowAreYouImage(IRandomAccessStream image)
        {
            var prevalentEmotion = await _emotionApi.GetPrevalentEmotion(image);
            if (prevalentEmotion != "Normal")
                return new DialogStepResult(String.Format("You are full of {0}. What's up?", prevalentEmotion.ToLower()), new DialogStep(ProcessResponseOnWhatsUp, DidntCatchResponse));
            else
                return new DialogStepResult(")))", new DialogStep(FinalResponse, FinalResponse));
        }

        private async Task<DialogStepResult> DidntCatchResponse(object arg)
        {
            return new DialogStepResult("Didn't catch that. What did you mean?", null);
        }

        protected async Task<DialogStepResult> FinalResponse(object arg)
        {
            return new DialogStepResult("Sorry, I'm busy. Let's talk later", new DialogStep(FinalResponse, FinalResponse));
        }

        public async Task<DialogStepResult> ProcessResponseOnHowAreYouText(string text)
        {
            var sentiment = await _sentimentAnalysisService.GetSentiment(text);

            if (sentiment < 0.3)
                return new DialogStepResult("What's up?", new DialogStep(ProcessResponseOnWhatsUp, DidntCatchResponse));
            if (sentiment > 0.9)
                return new DialogStepResult("Sounds great! Keep in the same spirit", new DialogStep(FinalResponse, FinalResponse));
            else
                return new DialogStepResult(")))", new DialogStep(FinalResponse, FinalResponse));
        }

        private async Task<DialogStepResult> ProcessResponseOnWhatsUp(string text)
        {
            var sentiment = await _sentimentAnalysisService.GetSentiment(text);

            if (sentiment < 0.3)
                return new DialogStepResult("Sounds bad... Be brave, we will cope with all of this", new DialogStep(ProcessResponseOnWhatsUp, DidntCatchResponse));
            if (sentiment > 0.9)
                return new DialogStepResult("Sounds not so bad. Was it a joke?", new DialogStep(FinalResponse, FinalResponse));
            else
                return new DialogStepResult("Oh, perfect", new DialogStep(FinalResponse, FinalResponse));
        }
    }
}
