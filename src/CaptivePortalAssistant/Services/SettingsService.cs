using CaptivePortalAssistant.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using CaptivePortalAssistant.Helpers;

namespace CaptivePortalAssistant.Services
{

    public class SettingsService : INotifyPropertyChanged
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        private static readonly ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;

        private SettingsService() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public static SettingsService Instance { get; } = new SettingsService();
        
        public DetectionMethod DetectionMethod
        {
            get => GetEnum(DetectionMethod.Android, SettingType.Roaming);
            set => SetEnum(value, SettingType.Roaming);
        }

        public AutomationOption DefaultAutomationOption
        {
            get => GetEnum(AutomationOption.Autologin, SettingType.Roaming);
            set => SetEnum(value, SettingType.Roaming);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(T value, SettingType type, [CallerMemberName] string property = null)
        {
            var settings = type == SettingType.Local ? LocalSettings : RoamingSettings;
            settings.Values[property] = value;
            OnPropertyChanged(property);
        }

        private static T Get<T>(T defaultValue, SettingType type, [CallerMemberName] string property = null)
        {
            var settings = type == SettingType.Local ? LocalSettings : RoamingSettings;
            var value = settings.Values[property];
            return value != null ? (T)value : defaultValue;
        }

        private void SetEnum<T>(T value, SettingType type, [CallerMemberName] string property = null)
        {
            var settings = type == SettingType.Local ? LocalSettings : RoamingSettings;
            settings.Values[property] = value.ToString();
            OnPropertyChanged(property);
        }

        private static T GetEnum<T>(T defaultValue, SettingType type, [CallerMemberName] string property = null) where T : struct
        {
            var settings = type == SettingType.Local ? LocalSettings : RoamingSettings;
            var value = settings.Values[property];
            return Enum.TryParse(value as string, out T result) ? result : defaultValue;
        }
    }
}
