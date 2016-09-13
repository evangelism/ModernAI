using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace LuisTalk.Models.Services
{
    public class DialogManager
    {
        private Dictionary<string, Dialog> _storage = new Dictionary<string, Dialog>();
        private Dialog _dialog;
        public Dialog _emptyDialog;

        public DialogManager(Dialog emptyDialog)
        {
            Check.Required<ArgumentNullException>(() => emptyDialog != null);

            _dialog = _emptyDialog = emptyDialog;
            emptyDialog.Initialize();
        }

        public string Initialize(string dialogName)
        {
            Check.Required<ArgumentNullException>(() => dialogName != null);

            if (!_storage.TryGetValue(dialogName, out _dialog))
                _dialog = _emptyDialog;

            return _dialog.Initialize();
        }

        public async Task<string> Process(string text)
        {
            Check.Required<ArgumentNullException>(() => text != null);

            return await _dialog.Process(text);
        }

        public async Task<string> Process(IRandomAccessStream image)
        {
            Check.Required<ArgumentNullException>(() => image != null);

            return await _dialog.Process(image);
        }

        public void AddDialog(Dialog dialog)
        {
            Check.Required<ArgumentNullException>(() => dialog != null);

            _storage[dialog.Name] = dialog;
        }
    }
}
