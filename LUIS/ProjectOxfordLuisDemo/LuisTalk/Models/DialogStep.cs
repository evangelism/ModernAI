using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace LuisTalk.Models
{
    public class DialogStep
    {
        private Func<string, Task<DialogStepResult>> _textProcessingAction;
        private Func<IRandomAccessStream, Task<DialogStepResult>> _imageProcessAction;

        public DialogStep(Func<string, Task<DialogStepResult>> textProcessingAction, Func<IRandomAccessStream, Task<DialogStepResult>> imageProcessingAction)
        {
            Check.Required<ArgumentNullException>(() => textProcessingAction != null);
            Check.Required<ArgumentNullException>(() => textProcessingAction != null);

            _textProcessingAction = textProcessingAction;
            _imageProcessAction = imageProcessingAction;
        }

        public async Task<DialogStepResult> Process(string text)
        {
            Check.Required<ArgumentNullException>(() => text != null);

            return await _textProcessingAction.Invoke(text);
        }

        public async Task<DialogStepResult> Process(IRandomAccessStream image)
        {
            Check.Required<ArgumentNullException>(() => image != null);

            return await _imageProcessAction(image);
        }
    }

    public class DialogStepResult
    {
        public string Response { get; private set; }
        public DialogStep NextStep { get; private set; }

        public DialogStepResult(string response, DialogStep nextStep)
        {
            Check.Required<ArgumentNullException>(() => response != null);

            Response = response;
            NextStep = nextStep;
        }
    }
}
