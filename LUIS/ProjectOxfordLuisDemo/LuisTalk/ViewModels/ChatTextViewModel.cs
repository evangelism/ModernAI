using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.ViewModels
{
    public class ChatTextViewModel: ChatItemViewModel
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                Set(() => Text, ref _text, value);
            }
        }
    }
}
