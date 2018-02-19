using CaptivePortalAssistant.Models;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using CaptivePortalAssistant.Helpers;

namespace CaptivePortalAssistant.Services
{
    public class ProfilesService : IDisposable
    {
        private const string DbFileName = "Profiles.db";
        private const string DbCollectionName = "profiles";

        private readonly LiteDatabase _db;
        private readonly LiteCollection<Profile> _dbCollection;
        private ExtendedObservableCollection<Profile> _profilesList;

        private ProfilesService()
        {
            var filename = Path.Combine(ApplicationData.Current.LocalFolder.Path, DbFileName);
            var password = "CaptivePortalAssistant"; // TODO: Encryption
            _db = new LiteDatabase($"Filename={filename}; Password={password}");
            _dbCollection = _db.GetCollection<Profile>(DbCollectionName);
        }

        public static ProfilesService Instance { get; } = new ProfilesService();

        public ExtendedObservableCollection<Profile> ProfilesList
        {
            get
            {
                if (_profilesList == null)
                    ProfilesList = new ExtendedObservableCollection<Profile>(_dbCollection.FindAll());
                return _profilesList;
            }
            private set
            {
                if (_profilesList != null)
                {
                    _profilesList.CollectionChanged -= ProfilesList_CollectionChanged;
                    _profilesList.ItemPropertyChanged -= ProfilesList_ItemPropertyChanged;
                }
                _profilesList = value;
                _profilesList.CollectionChanged += ProfilesList_CollectionChanged;
                _profilesList.ItemPropertyChanged += ProfilesList_ItemPropertyChanged;
            }
        }

        public LiteCollection<Profile> ProfilesDbCollection => _dbCollection;

        public void Dispose()
        {
            _db?.Dispose();
        }

        private void ProfilesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Profile item in e.NewItems)
                        _dbCollection.Upsert(item);
                    _dbCollection.EnsureIndex(x => x.Ssid);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Profile item in e.OldItems)
                        _dbCollection.Delete(item.Id);
                    break;
            }
        }

        private void ProfilesList_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _dbCollection.Update(sender as Profile);
            _dbCollection.EnsureIndex(x => x.Ssid);
        }

        public async Task Import(StorageFile storageFile)
        {
            var json = await FileIO.ReadTextAsync(storageFile);
            var newProfiles = JsonConvert.DeserializeObject<List<Profile>>(json,
                JsonStorage.GetJsonSerializerSettings());
            ProfilesList.Clear();
            _db.DropCollection(DbCollectionName);
            foreach (var newProfile in newProfiles)
                ProfilesList.Add(newProfile);
        }

        public async Task Export(StorageFile storageFile)
        {
            var jsonSerializerSettings = JsonStorage.GetJsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            var json = JsonConvert.SerializeObject(ProfilesList, jsonSerializerSettings);
            await FileIO.WriteTextAsync(storageFile, json);
        }

        public bool ExistProfile(string ssid)
        {
            return ProfilesList.Any(p => p.Ssid == ssid);
        }

        public void AddProfile(Profile profile)
        {
            if (ExistProfile(profile.Ssid))
            {
                var oldProfile = ProfilesList.First(p => p.Ssid == profile.Ssid);
                var oldProfileIndex = ProfilesList.IndexOf(oldProfile);
                if (oldProfile.AutomationOption != AutomationOption.Global &&
                    profile.AutomationOption == AutomationOption.Global)
                {
                    profile.AutomationOption = oldProfile.AutomationOption;
                }
                ProfilesList[oldProfileIndex] = profile;
            }
            else
            {
                ProfilesList.Add(profile);
            }
        }
    }
}
