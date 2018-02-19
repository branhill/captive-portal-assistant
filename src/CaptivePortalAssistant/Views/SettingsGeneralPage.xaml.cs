using CaptivePortalAssistant.Services;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CaptivePortalAssistant.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsGeneralPage : Page
    {
        public string AppDescription
        {
            get
            {
                var package = Package.Current;
                var version = package.Id.Version;
                return $"{package.DisplayName} {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }

        public SettingsService SettingsStorageService => SettingsService.Instance;

        public SettingsGeneralPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) { }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Frame == Window.Current.Content as Frame && Window.Current.Bounds.Width >= 720)
                Frame?.GoBack();
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker
            {
                FileTypeFilter = { ".json" }
            };
            var storageFile = await fileOpenPicker.PickSingleFileAsync();

            if (storageFile == null) return;
            var isSuccess = true;
            try
            {
                await ProfilesService.Instance.Import(storageFile);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            var contentDialog = new ContentDialog
            {
                Title = "Import profiles",
                Content = isSuccess
                    ? "Imported successfully."
                    : "Import failed, please try again.",
                CloseButtonText = "Ok"
            };
            await contentDialog.ShowAsync();
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedFileName = "Captive Portal Assistant Profiles",
                FileTypeChoices =
                {
                    { "JSON", new List<string> { ".json" } }
                }
            };
            var storageFile = await fileSavePicker.PickSaveFileAsync();

            if (storageFile == null) return;
            CachedFileManager.DeferUpdates(storageFile);
            var isSuccess = true;
            try
            {
                await ProfilesService.Instance.Export(storageFile);
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            var fileUpdateStatus = await CachedFileManager.CompleteUpdatesAsync(storageFile);

            var contentDialog = new ContentDialog
            {
                Title = "Export profiles",
                Content = isSuccess && fileUpdateStatus == FileUpdateStatus.Complete
                    ? "Exported successfully."
                    : "Export failed, please try again.",
                CloseButtonText = "Ok"
            };
            await contentDialog.ShowAsync();
        }

        private async void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsforwebsites"));
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.msftconnecttest.com/connecttest.txt?defaultTest"));
        }
    }
}
