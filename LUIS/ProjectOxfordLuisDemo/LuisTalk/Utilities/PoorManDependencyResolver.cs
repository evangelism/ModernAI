using LuisTalk.Models.ConcreteDialogs;
using LuisTalk.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.Utilities
{
    public static class PoorManDependencyResolver
    {
        // singleton
        private static DialogManager _dialogManager;
        public static DialogManager DialogManager { get { return _dialogManager; } }
        public static VisionApi VisionApi { get; } = new VisionApi(@"a702f8e1a0514d22bb5e4fa16c7b50c7");
        public static SpellCheckerApi SpellCheckerApi { get; } = new SpellCheckerApi(@"1bf22e53df0444a8bf9f89f370801eb1");
        public static LuisService YouAreLateLuisService { get; } = new LuisService(@"5d6e389d-9dca-4fcb-8ba5-485e402c3d8c", @"3998a629d431410d885d69304ce074e7");
        public static SentimentAnalysisService SentimentAnalysisService { get; } = new SentimentAnalysisService(@"jSxiVY4KmFmweDsApV0ZQ9vhMROUK90Dk7ZqgluEKTs");
        public static EmotionApi EmotionApi { get; } = new EmotionApi(@"870fd342c5a449e49c5ff2a2256b89de");
        public static LuisService GuessLuisService { get; } = new LuisService("ca44e8dc-702e-43b6-b755-aa659bd16ad2", "3998a629d431410d885d69304ce074e7");

        static PoorManDependencyResolver()
        {
            _dialogManager = new DialogManager(new EmptyDialog());
            _dialogManager.AddDialog(new HowAreYouDialog(SentimentAnalysisService, EmotionApi));
            _dialogManager.AddDialog(new NumberGuessDialog(GuessLuisService));
            _dialogManager.AddDialog(new YouAreLateDialog(YouAreLateLuisService));
        }
    }
}
