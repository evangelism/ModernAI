using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace LuisTalk.ViewModels
{
    public class ChatImageViewModel: ChatItemViewModel
    {
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                Set(() => Image, ref _image, value);
            }
        }
    }
}
