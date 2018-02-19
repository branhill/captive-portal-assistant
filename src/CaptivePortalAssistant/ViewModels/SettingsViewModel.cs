using CaptivePortalAssistant.Models;
using CaptivePortalAssistant.Services;
using CaptivePortalAssistant.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace CaptivePortalAssistant.ViewModels
{
    public class SettingsViewModel : NotifyModel
    {
        private ObservableCollection<SettingItem> _settingsList;
        private ExtendedObservableCollection<Profile> _profilesList;
        private SettingItem _selectedSettingItem;
        private Profile _selectedProfile;

        public SettingsViewModel()
        {
            SettingsList = new ObservableCollection<SettingItem>
            {
                new SettingItem("General", "\xE713", typeof(SettingsGeneralPage))
            };
            SelectedSettingItem = SettingsList.First();
            ProfilesList = ProfilesService.Instance.ProfilesList;
        }

        public ObservableCollection<SettingItem> SettingsList
        {
            get => _settingsList;
            private set => Set(ref _settingsList, value);
        }

        public ExtendedObservableCollection<Profile> ProfilesList
        {
            get => _profilesList;
            private set => Set(ref _profilesList, value);
        }

        public SettingItem SelectedSettingItem
        {
            get => _selectedSettingItem;
            set
            {
                Set(ref _selectedSettingItem, value);
                if (value != null)
                    PreSelectedItem = value;
            }
        }

        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                Set(ref _selectedProfile, value);
                if (value != null)
                    PreSelectedItem = value;
                if (value == null && SelectedSettingItem == null)
                {
                    SelectedSettingItem = SettingsList.First();
                }
            }
        }

        public object PreSelectedItem { get; private set; }

        public void AddProfile()
        {
            const string baseName = "New";
            var newProfile = new Profile
            {
                Ssid = ProfilesList.All(p => p.Ssid != baseName) ? baseName : GetNewName(baseName)
            };

            ProfilesList.Add(newProfile);
            SelectedProfile = newProfile;
        }

        public void DeleteSelectedProfile()
        {
            var index = ProfilesList.IndexOf(SelectedProfile);

            ProfilesList.Remove(SelectedProfile);

            if (ProfilesList.Count <= index)
                index--;
            if (index >= 0)
            {
                SelectedProfile = ProfilesList[index];
            }
            else
            {
                SelectedProfile = null;
                SelectedSettingItem = SettingsList.First();
            }
        }

        public void DeleteAllSelectedProfiles(IList<Profile> selectedProfiles)
        {
            foreach (var selectedProfile in selectedProfiles)
            {
                ProfilesList.Remove(selectedProfile);
            }
        }

        public void DuplicateSelectedProfile()
        {
            var newProfile = SelectedProfile.Clone();

            var match = Regex.Match(newProfile.Ssid, @"(.*)\((\d+)\)$");
            var name = match.Success ? match.Groups[1].Value : newProfile.Ssid;
            var num = match.Success ? int.Parse(match.Groups[2].Value) : 1;
            newProfile.Ssid = GetNewName(name, num);

            ProfilesList.Add(newProfile);
            SelectedProfile = newProfile;
        }

        private string GetNewName(string name, int startNum = 1)
        {
            while (ProfilesList.Any(x => x.Ssid == $"{name}({startNum})"))
                startNum++;
            return $"{name}({startNum})";
        }
    }
}