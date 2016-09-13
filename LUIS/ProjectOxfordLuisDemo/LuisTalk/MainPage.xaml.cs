using LuisTalk.Models;
using LuisTalk.Models.ConcreteDialogs;
using LuisTalk.Models.Services;
using LuisTalk.Utilities;
using LuisTalk.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LuisTalk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ChatPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && ViewModel.SendCommand.CanExecute(null))
            {
                ViewModel.SendCommand.Execute(null);
            }
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
            {
                tbInput.Focus(FocusState.Programmatic);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new ChatPageViewModel(PoorManDependencyResolver.SpellCheckerApi, PoorManDependencyResolver.VisionApi, PoorManDependencyResolver.DialogManager);

            base.OnNavigatedTo(e);
        }

        private void ItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            swMessages.ChangeView(swMessages.HorizontalOffset, swMessages.ScrollableHeight+1, 1);
        }

        private void tbInput_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            ViewModel.InputText = sender.Text;
        }
    }
}
