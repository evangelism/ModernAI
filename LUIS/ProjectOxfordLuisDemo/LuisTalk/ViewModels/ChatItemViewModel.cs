using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.ViewModels
{
    public class ChatItemViewModel: ViewModelBase
    {
        private DateTime _sentDateTime;
        public DateTime SentDateTime
        {
            get { return _sentDateTime; }
            set
            {
                Set(() => SentDateTime, ref _sentDateTime, value);
            }
        }

        private SenderSide _senderSide;
        public SenderSide SenderSide
        {
            get { return _senderSide; }
            set
            {
                Set(() => SenderSide, ref _senderSide, value);
            }
        }
    }
}
