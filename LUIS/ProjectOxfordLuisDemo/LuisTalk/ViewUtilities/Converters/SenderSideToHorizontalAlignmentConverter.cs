using LuisTalk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LuisTalk.ViewUtilities.Converters
{
    public class SenderSideToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(HorizontalAlignment) && value.GetType() == typeof(SenderSide))
            {
                switch ((SenderSide)value)
                {
                    case SenderSide.Local:
                        return HorizontalAlignment.Right;
                    case SenderSide.Remote:
                        return HorizontalAlignment.Left;
                }
            }

            throw new ArgumentOutOfRangeException("Converter type mismatch has occured.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
