using CaptivePortalAssistant.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CaptivePortalAssistant
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private readonly UISettings _uiSettings = new UISettings();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                rootFrame.ContentTransitions = new TransitionCollection
                {
                    new EntranceThemeTransition
                    {
                        FromVerticalOffset = -28
                    }
                };

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(WebViewPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }

        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                rootFrame.ContentTransitions = new TransitionCollection
                {
                    new EntranceThemeTransition
                    {
                        FromVerticalOffset = -28
                    }
                };

                Window.Current.Content = rootFrame;
            }

            // Default navigation
            rootFrame.Navigate(typeof(WebViewPage), e);

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            // Register a global back event handler. This can be registered on a per-page-bases if you only have a
            // subset of your pages that needs to handle back or if you want to do page-specific logic before deciding
            // to navigate back on those pages.
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            _uiSettings.ColorValuesChanged += App_ColorValuesChanged;

            SetAppViewBackButtonVisibility();
            SetWindowTitleBarColor();
        }


        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            SetAppViewBackButtonVisibility();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private static async void App_ColorValuesChanged(UISettings sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                SetWindowTitleBarColor);
        }

        private static void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) return;

            // Navigate back if possible, and if the event has not 
            // already been handled.
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private static void SetAppViewBackButtonVisibility()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) return;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private static void SetWindowTitleBarColor()
        {
            if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView")) return;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar == null) return;

            titleBar.BackgroundColor = titleBar.InactiveBackgroundColor = titleBar.ButtonBackgroundColor =
                titleBar.ButtonInactiveBackgroundColor =
                    (Color)Current.Resources["SystemChromeMediumColor"];
            titleBar.ForegroundColor = titleBar.ButtonForegroundColor = titleBar.ButtonHoverForegroundColor =
                titleBar.ButtonPressedForegroundColor =
                    (Color)Current.Resources["SystemBaseHighColor"];
            titleBar.ButtonHoverBackgroundColor = GetButtonBackgroundColor("SystemChromeMediumColor", false);
            titleBar.ButtonPressedBackgroundColor = GetButtonBackgroundColor("SystemChromeMediumColor", true);
        }

        private static Color GetButtonBackgroundColor(string baseColorResource, bool isPressed)
        {
            var baseColor = (Color)Current.Resources[baseColorResource];
            const byte deltaHover = 23;
            const byte deltaPressed = 46;
            var delta = isPressed ? deltaPressed : deltaHover;
            // Light theme
            return baseColor.R > 128
                ? Color.FromArgb(255, (byte)(baseColor.R - delta), (byte)(baseColor.G - delta),
                    (byte)(baseColor.B - delta))
                : Color.FromArgb(255, (byte)(baseColor.R + delta), (byte)(baseColor.G + delta),
                    (byte)(baseColor.B + delta));
        }
    }
}
