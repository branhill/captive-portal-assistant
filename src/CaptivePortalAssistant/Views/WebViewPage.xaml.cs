using CaptivePortalAssistant.Helpers;
using CaptivePortalAssistant.Models;
using CaptivePortalAssistant.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace CaptivePortalAssistant.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebViewPage : Page
    {
        private readonly DispatcherTimer _checkConnectTimer;
        private readonly SymbolIcon _symbolIconCancel = new SymbolIcon(Symbol.Cancel);
        private readonly SymbolIcon _symbolIconRefresh = new SymbolIcon(Symbol.Refresh);
        private Profile _profile;
        private string _currentSsid;
        private bool _protocolactivation;

        public WebViewPage()
        {
            InitializeComponent();

            _checkConnectTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 5)
            };
            _checkConnectTimer.Tick += CheckConnectTimerTick;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ProtocolActivatedEventArgs parameter)
            {
                MainWebView.Navigate(parameter.Uri);
                if (parameter.Uri.Query.Contains("defaultTest"))
                {
                    _protocolactivation = false;
                    var contentDialog = new ContentDialog
                    {
                        Title = "Association",
                        Content = "Associated successfully.",
                        CloseButtonText = "Ok"
                    };
                    await contentDialog.ShowAsync();
                }
                else
                {
                    _protocolactivation = true;
                }
            }
            else if (MainWebView.Source == null)
            {
                MainWebView.Navigate(new Uri("http://www.msftconnecttest.com/connecttest.txt"));
                _protocolactivation = false;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CaptivePortalDetector.DefaultDetectionMethod = SettingsService.Instance.DetectionMethod;

            _currentSsid = await WifiInfo.GetSsid();
            _profile = ProfilesService.Instance.ProfilesDbCollection.FindOne(x => x.Ssid == _currentSsid);

            _checkConnectTimer.Start();
            UpdateButtonStates();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _checkConnectTimer.Stop();
        }

        private void MainWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            AddressTextbox.Text = args.Uri.ToString();
            RefreshStopButton.Icon = _symbolIconCancel;
            LoadingProgressBar.IsIndeterminate = true;
            UpdateButtonStates();
        }

        private async void MainWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            RefreshStopButton.Icon = _symbolIconRefresh;
            LoadingProgressBar.IsIndeterminate = false;
            UpdateButtonStates();

            await CheckConnectivity();
            if (_protocolactivation)
            {
                await Task.Delay(300);
                await AutoOpration();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWebView.CanGoBack)
                MainWebView.GoBack();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWebView.CanGoForward)
                MainWebView.GoForward();
        }

        private void RefreshStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadingProgressBar.IsIndeterminate)
            {
                MainWebView.Stop();
                MainWebView_NavigationCompleted(null, null);
            }
            else
            {
                MainWebView.Refresh();
            }
        }

        private void AddressTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;

            var url = AddressTextbox.Text.Trim();
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "http://" + url;
            }
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                MainWebView.Navigate(new Uri(url));
        }

        private void AddressTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressTextbox.SelectAll();
        }

        private void AddressTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            AddressTextbox.SelectionLength = 0;
        }

        private async void FillButton_Click(object sender, RoutedEventArgs e)
        {
            await InsertFillScript(false);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await InsertFillScript(true);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await MainWebView.InvokeScriptAsync("eval", new[] { ScriptBuilder.GetSaveScript() });

            List<List<ProfileField>> forms;
            try
            {
                forms = JsonConvert.DeserializeObject<List<List<ProfileField>>>(result,
                    JsonStorage.GetJsonSerializerSettings());
            }
            catch (JsonException)
            {
                await ShowDialog("Error", "Cannot save form.");
                return;
            }

            if (!forms.Any())
            {
                await ShowDialog("Forms not found", "We didn't find any forms that can be saved.");
                return;
            }

            var newSSid = await WifiInfo.GetSsid();
            if (!string.IsNullOrWhiteSpace(newSSid))
                _currentSsid = newSSid;
            var saveFormContentDialog = new SaveFormContentDialog(_currentSsid, forms);
            await saveFormContentDialog.ShowAsync();
            Page_Loaded(null, null);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }


        private async Task CheckConnectivity()
        {
            if (_protocolactivation)
            {
                var isCaptivePortal = await CaptivePortalDetector.IsCaptivePortalAsync();
                if (!isCaptivePortal)
                    Application.Current.Exit();
            }
            else if (!Frame.ForwardStack.Any())
            {
                Frame.Navigate(typeof(SettingsPage));
            }
        }

        private async void CheckConnectTimerTick(object sender, object e)
        {
            await CheckConnectivity();
        }

        private async Task AutoOpration()
        {
            if (_profile == null) return;

            var automationOption = _profile.AutomationOption == AutomationOption.Global
                ? SettingsService.Instance.DefaultAutomationOption
                : _profile.AutomationOption;

            switch (automationOption)
            {
                case AutomationOption.Autologin:
                    await InsertFillScript(true);
                    break;
                case AutomationOption.Autofill:
                    await InsertFillScript(false);
                    break;
            }
        }

        private async Task InsertFillScript(bool isLoginEnabled)
        {
            if (_profile.Fields != null && _profile.Fields.Any())
            {
                await MainWebView.InvokeScriptAsync("eval",
                    new[] { ScriptBuilder.GetFillScript(_profile, isLoginEnabled) });
            }
        }

        private void UpdateButtonStates()
        {
            if (LoadingProgressBar.IsIndeterminate)
            {
                FillButton.IsEnabled = LoginButton.IsEnabled = SaveButton.IsEnabled =
                    false;
            }
            else
            {
                FillButton.IsEnabled = LoginButton.IsEnabled =
                    _profile?.Fields != null;
                SaveButton.IsEnabled = true;
            }
        }

        private static async Task ShowDialog(string title, string content)
        {
            var contentDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };
            await contentDialog.ShowAsync();
        }
    }
}
