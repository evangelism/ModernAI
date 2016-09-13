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
    public class Dialog
    {
        public string Name { get; private set; }
        public DialogStep CurrentStep { get; protected set; }

        public Dialog(string name)
        {
            Check.Required<ArgumentNullException>(() => name != null);

            Name = name;
        }

        public async Task<string> Process(string text)
        {
            Check.Required<ArgumentNullException>(() => text != null);

            var result = await CurrentStep.Process(text);
            if (result.NextStep != null)
                CurrentStep = result.NextStep;
            return result.Response;
        }

        public async Task<string> Process(IRandomAccessStream image)
        {
            Check.Required<ArgumentNullException>(() => image != null);

            var result = await CurrentStep.Process(image);
            if (result.NextStep != null)
                CurrentStep = result.NextStep;
            return result.Response;
        }

        public virtual string Initialize()
        {
            throw new ArgumentNullException();
        }
    }
}
