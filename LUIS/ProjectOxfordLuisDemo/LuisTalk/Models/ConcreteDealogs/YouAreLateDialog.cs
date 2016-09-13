using LuisTalk.Models.Services;
using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace LuisTalk.Models.ConcreteDialogs
{
    public class YouAreLateDialog: Dialog
    {
        private LuisService _luisService;

        public YouAreLateDialog(LuisService luisService)
            :base("YouAreLate")
        {
            Check.Required<ArgumentNullException>(() => luisService != null);

            _luisService = luisService;
        }

        public override string Initialize()
        {
            CurrentStep = new DialogStep(ProcessResponseOnYouAreLate, DidntCatchResponse);
            return "Have you seen what time it is? We are late!\nWhere are you?";
        }

        private Task<DialogStepResult> DidntCatchResponse(object arg)
        {
            return Task.FromResult(new DialogStepResult("Didn't catch that. What did you mean?", null));
        }

        protected Task<DialogStepResult> FinalResponse(object arg)
        {
            return Task.FromResult(new DialogStepResult("Hurry up! Let's talk when you get here", new DialogStep(FinalResponse, FinalResponse)));
        }

        private async Task<DialogStepResult> ProcessResponseOnYouAreLate(string text)
        {
            var luisResult = await _luisService.GetIntent(text);
            
            
            switch(luisResult.Intent)
            {
                case "BeSoon":
                    if (luisResult.Entities.ContainsKey("builtin.datetime.time"))
                        return new DialogStepResult(String.Format("{0}, no more!", luisResult.Entities["builtin.datetime.time"]), new DialogStep(ProcessResponseOnHurryUp, DidntCatchResponse));
                    else
                        return new DialogStepResult("Ok... No more!", new DialogStep(ProcessResponseOnHurryUp, DidntCatchResponse));
                case "AskForgiveness":
                    return new DialogStepResult("No more sorries! Hurry up!", new DialogStep(ProcessResponseOnYouAreLate, DidntCatchResponse));
                case "AlreadyHere":
                    return new DialogStepResult("Really? And where were you all that time? I'm on the second floor", new DialogStep(FinalResponse, FinalResponse));
                default:
                    return await DidntCatchResponse(null);
            }
        }

        private async Task<DialogStepResult> ProcessResponseOnHurryUp(string text)
        {
            var luisResult = await _luisService.GetIntent(text);

            switch (luisResult.Intent)
            {
                case "AlreadyHere":
                    return new DialogStepResult("Finally! I'm on the second floor", new DialogStep(FinalResponse, FinalResponse));
                case "AskForgiveness":
                    return new DialogStepResult("No more sorries! Hurry up!", new DialogStep(ProcessResponseOnHurryUp, DidntCatchResponse));
                default:
                    return await DidntCatchResponse(null);
            }
        }
    }
}
