using LuisTalk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LuisTalk.ViewUtilities.DataTemplateSelectors
{
    public class ChatItemDataTemplateSelector:DataTemplateSelector
    {
        public DataTemplate ChatImageDataTemplate { get; set; }
        public DataTemplate ChatTextDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ChatImageViewModel)
                return ChatImageDataTemplate;
            if (item is ChatTextViewModel)
                return ChatTextDataTemplate;

            throw new ArgumentOutOfRangeException("Unknown item type");
        }
    }
}
