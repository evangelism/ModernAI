using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LuisTalk.Models.ConcreteDialogs;
using LuisTalk.Models.Services;
using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

namespace LuisTalk.ViewModels
{
    public class ChatPageViewModel: ViewModelBase
    {
        private SpellCheckerApi _spellCheckerApi;
        private VisionApi _visionApi;
        private DialogManager _dialogManager;

        public ChatPageViewModel(SpellCheckerApi spellCheckerApi, VisionApi visionApi, DialogManager dialogManager)
        {
            Check.Required<ArgumentNullException>(() => spellCheckerApi != null);
            Check.Required<ArgumentNullException>(() => visionApi != null);
            Check.Required<ArgumentNullException>(() => dialogManager != null);

            _spellCheckerApi = spellCheckerApi;
            _visionApi = visionApi;
            _dialogManager = dialogManager;
        }

        private ObservableCollection<ChatItemViewModel> _chatItems = new ObservableCollection<ChatItemViewModel>();
        public ObservableCollection<ChatItemViewModel> ChatItems
        {
            get { return _chatItems; }
            private set
            {
                Set(() => ChatItems, ref _chatItems, value);
            }
        }

        private string _inputText = String.Empty;
        public string InputText
        {
            get { return _inputText; }
            set
            {
                Set(() => InputText, ref _inputText, value);
            }
        }

        public RelayCommand SendCommand => new RelayCommand(SendCommandExecute);

        public RelayCommand PickPhotoCommand => new RelayCommand(PickPhotoExecute);

        private async void PickPhotoExecute()
        {
            try
            {
                var file = await GetImageFile();
                if (file == null)
                    return;

                var bitmap = new BitmapImage();
                var thumbnail = await _visionApi.GetThumbnailImage(await file.OpenReadAsync(), 500, 500);
                await bitmap.SetSourceAsync(thumbnail);
                SendImage(bitmap, SenderSide.Local);

                var response = await _dialogManager.Process(await file.OpenReadAsync());
                SendText(response, SenderSide.Remote);
            }
            catch (Exception e)
            {
                SendText(e.ToString(), SenderSide.Remote);
            }
        }

        private static async Task<Windows.Storage.StorageFile> GetImageFile()
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.CommitButtonText = "Choose";
            var file = await filePicker.PickSingleFileAsync();
            return file;
        }

        private async void SendCommandExecute()
        {
            try
            {
                string name;
                if (IsStartCommand(InputText, out name))
                {
                    var initializeReposnse = _dialogManager.Initialize(name);
                    SendText(initializeReposnse, SenderSide.Remote);
                    InputText = String.Empty;
                    return;
                }

                //var text = await PoorManDependencyResolver.SpellCheckerApi.CorrectAsync(InputText);
                SendText(InputText, SenderSide.Local);
                InputText = String.Empty;

                var response = await _dialogManager.Process(text);
                SendText(response, SenderSide.Remote);
            }
            catch (Exception e)
            {
                SendText(e.ToString(), SenderSide.Remote);
            }
        }

        private bool IsStartCommand(string text, out string name)
        {
            if(!text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
            {
                name = null;
                return false;
            }
            else
            {
                name = text.Split(' ')[1];
                return true;
            }
        }

        private void SendText(string text, SenderSide side = SenderSide.Local)
        {
            var chatText = new ChatTextViewModel()
            {
                Text = text,
                SentDateTime = DateTime.Now,
                SenderSide = side
            };
            ChatItems.Add(chatText);
        }

        private void SendImage(BitmapSource image, SenderSide side = SenderSide.Local)
        {
            var chatImage = new ChatImageViewModel()
            {
                Image = image,
                SentDateTime = DateTime.Now,
                SenderSide = side
            };
            ChatItems.Add(chatImage);
        }
    }

    public enum SenderSide
    {
        Local,
        Remote
    }
}
