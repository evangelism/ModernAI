using LuisTalk.Models.Services;
using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.Models.ConcreteDialogs
{
    public class NumberGuessDialog: Dialog
    {
        private int _unknownNumber;
        private int _retriesLeft;
        private LuisService _luisService;
        private string[] _noResponses = new string[] {"Another one?", "No. Ideas?", "Try another one", "Perhaps, something bigger?", "Smaller one?", "Thank you. Just a few wrong guesses and I'm the winner!", "Sorry, but not that one..."};
        private List<int> _mentionedNumbers = new List<int>();

        public NumberGuessDialog(LuisService luisService)
            :base("GuessANumber")
        {
            Check.Required<ArgumentNullException>(() => luisService != null);

            _luisService = luisService;
        }

        public override string Initialize()
        {
            _unknownNumber = (new Random()).Next(9);
            _retriesLeft = 5;
            _mentionedNumbers.Clear();
            CurrentStep = new DialogStep(ProcessGuessResponse, DidntCatchResponse);

            return "Ok. Let's start. Guess a number from 0 to 9";
        }

        private async Task<DialogStepResult> ProcessGuessResponse(string text)
        {
            string guessString = null;
            var result = await _luisService.GetIntent(text);
            result?.Entities.TryGetValue("builtin.number", out guessString);
            int? guess = ConvertToNumber(guessString);
            if (guess == null)
                return await DidntCatchResponse(null);

            if (_retriesLeft <= 0)
                return new DialogStepResult("This one was the last. Who you are? Who? You are the looooser!", new DialogStep(FinalResponse, FinalResponse));

            if (guess == _unknownNumber)
                return new DialogStepResult("You got it. My congratulation!!!", new DialogStep(FinalResponse, FinalResponse));
            else if (_mentionedNumbers.Contains(guess.Value))
                return new DialogStepResult("Oh, you have already called this one. Try again", new DialogStep(ProcessGuessResponse, FinalResponse));
            else
            {
                var response = _noResponses[(new Random()).Next(_noResponses.Length)];
                _mentionedNumbers.Add(guess.Value);
                _retriesLeft--;
                return new DialogStepResult(response, new DialogStep(ProcessGuessResponse, DidntCatchResponse));
            }
        }

        private async Task<DialogStepResult> DidntCatchResponse(object arg)
        {
            return new DialogStepResult("Didn't catch that. Can you repeat?", null);
        }

        protected async Task<DialogStepResult> FinalResponse(object arg)
        {
            return new DialogStepResult("You won. I give up...", new DialogStep(FinalResponse, FinalResponse));
        }

        private int? ConvertToNumber(string value)
        {
            if (value == null)
                return null;

            switch (value)
            {
                case "zero":
                case "0":
                    return 0;
                case "one":
                case "1":
                    return 1;
                case "two":
                case "2":
                    return 2;
                case "three":
                case "3":
                    return 3;
                case "four":
                case "4":
                    return 4;
                case "five":
                case "5":
                    return 5;
                case "six":
                case "6":
                    return 6;
                case "seven":
                case "7":
                    return 7;
                case "eight":
                case "8":
                    return 8;
                case "nine":
                case "9":
                    return 9;
                default:
                    return null;
            }
        }
    }
}
