using CaptivePortalAssistant.Models;
using CaptivePortalAssistant.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CaptivePortalAssistant.Views
{
    public sealed partial class SaveFormContentDialog : ContentDialog
    {
        private ProfilesService _profilesService;

        public SaveFormContentDialog(string ssid, List<List<ProfileField>> forms)
        {
            InitializeComponent();

            Ssid = ssid;
            var maxCount = 0;
            List<ProfileField> loginForm = null;
            foreach (var fields in forms)
            {
                var count = fields.Sum(f => f.Value.Length);
                if (count < maxCount) continue;
                maxCount = count;
                loginForm = fields;
            }
            Fields = new ExtendedObservableCollection<ProfileField>(loginForm);
        }

        public string Ssid { get; set; }
        public ExtendedObservableCollection<ProfileField> Fields { get; set; }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            _profilesService = ProfilesService.Instance;
            if (_profilesService.ExistProfile(Ssid))
                ReplaceWarningTextBlock.Visibility = Visibility.Visible;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _profilesService.AddProfile(new Profile { Ssid = Ssid, Fields = Fields });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((AppBarButton)sender).DataContext as ProfileField;
            Fields.Remove(item);
        }

    }
}
