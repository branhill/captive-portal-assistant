using CaptivePortalAssistant.Models;
using CaptivePortalAssistant.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CaptivePortalAssistant.Helpers;

namespace CaptivePortalAssistant.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsProfilePage : Page
    {
        public SettingsProfilePage()
        {
            InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (SettingsViewModel)e.Parameter;

            DataContext = this;

            if (Frame == Window.Current.Content as Frame)
            {
                BottomCommandBar.Visibility = Visibility.Visible;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Frame == Window.Current.Content as Frame && Window.Current.Bounds.Width >= 720)
                Frame?.GoBack();
        }

        private void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSelectedProfile();
            Frame?.GoBack();
        }

        private void AddFieldButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedProfile.Fields.Add(new ProfileField());
        }

        private void DeleteFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((AppBarButton)sender).DataContext as ProfileField;
            ViewModel.SelectedProfile.Fields.Remove(item);
        }

        private async void GetProfileNameButton_Click(object sender, RoutedEventArgs e)
        {
            var ssid = await WifiInfo.GetSsid();
            if (!string.IsNullOrEmpty(ssid))
            {
                ViewModel.SelectedProfile.Ssid = ssid;
            }
            else
            {
                var noWifiDialog = new ContentDialog
                {
                    Title = "No WiFi connected",
                    Content = "Cannot get current WiFi network's SSID.",
                    PrimaryButtonText = "Ok"
                };
                await noWifiDialog.ShowAsync();
            }
        }
    }
}
