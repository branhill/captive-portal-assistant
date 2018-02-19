using CaptivePortalAssistant.Models;
using CaptivePortalAssistant.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CaptivePortalAssistant.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        public SettingsViewModel ViewModel { get; set; } = new SettingsViewModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateActionButtonStatus();
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != DefaultState) return;
            switch (ViewModel.PreSelectedItem)
            {
                case SettingItem settingItem:
                    ViewModel.SelectedSettingItem = settingItem;
                    break;
                case Profile profile:
                    ViewModel.SelectedProfile = profile;
                    break;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Only be used for narrow state
            if (AdaptiveStates.CurrentState != NarrowState)
                return;

            if (sender == SettingsListView)
            {
                ViewModel.SelectedSettingItem = (SettingItem)e.ClickedItem;
                Frame.Navigate(ViewModel.SelectedSettingItem.Page);
            }
            else if (sender == ProfilesListView)
            {
                ViewModel.SelectedProfile = (Profile)e.ClickedItem;
                Frame.Navigate(typeof(SettingsProfilePage), ViewModel);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateActionButtonStatus();
            if (!e.AddedItems.Any() || ((ListView)sender).SelectionMode == ListViewSelectionMode.Multiple)
                return;
            var otherListView = sender == SettingsListView ? ProfilesListView : SettingsListView;
            otherListView.SelectedItem = null;

            // Only be used for default state
            if (AdaptiveStates.CurrentState != DefaultState)
                return;
            if (sender == SettingsListView)
            {
                DetailFrame.Navigate(ViewModel.SelectedSettingItem.Page);
            }
            else if (sender == ProfilesListView && (DetailFrame.CurrentSourcePageType != typeof(SettingsProfilePage) || DetailFrame.Content == null))
            {
                DetailFrame.Navigate(typeof(SettingsProfilePage), ViewModel);
            }
        }

        private void UpdateActionButtonStatus()
        {
            SelectItemsButton.IsEnabled = ViewModel.ProfilesList.Any();
            DuplicateItemButton.IsEnabled = DeleteItemButton.IsEnabled = ViewModel.SelectedProfile != null;
            DeleteItemsButton.IsEnabled = ProfilesListView.SelectedItems.Any();
        }

        private void SelectItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSettingItem = null;
            VisualStateManager.GoToState(this, MultipleSelectionState.Name, true);
            DetailFrame.Content = null;
        }

        private void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteAllSelectedProfiles(ProfilesListView.SelectedItems.Cast<Profile>().ToList());
        }

        private void CancelSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this,
                AdaptiveStates.CurrentState == DefaultState ? SelectionDefaultState.Name : SelectionNarrowState.Name,
                true);

            switch (ViewModel.PreSelectedItem)
            {
                case SettingItem settingItem:
                    ViewModel.SelectedSettingItem = settingItem;
                    break;
                case Profile profile:
                    if (ViewModel.ProfilesList.Any(p => p == profile))
                    {
                        ViewModel.SelectedProfile = profile;
                    }
                    else if (ViewModel.ProfilesList.Any())
                    {
                        ViewModel.SelectedProfile = ViewModel.ProfilesList.First();
                    }
                    else
                    {
                        ViewModel.SelectedSettingItem = ViewModel.SettingsList.First();
                    }
                    break;
            }
        }

    }
}
