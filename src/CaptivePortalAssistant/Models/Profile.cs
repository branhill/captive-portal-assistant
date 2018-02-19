using Newtonsoft.Json;
using System;

namespace CaptivePortalAssistant.Models
{
    public class Profile : NotifyModel
    {
        private int _id;
        private string _ssid;
        private AutomationOption _automationOption;
        private ExtendedObservableCollection<ProfileField> _fields;
        private string _submitButton;

        public Profile()
        {
            AutomationOption = AutomationOption.Global;
            Fields = new ExtendedObservableCollection<ProfileField>();
        }

        private void FieldsOnItemChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Fields.Inner");
        }

        [JsonIgnore]
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string Ssid
        {
            get => _ssid;
            set => Set(ref _ssid, value);
        }

        public AutomationOption AutomationOption
        {
            get => _automationOption;
            set => Set(ref _automationOption, value);
        }

        public ExtendedObservableCollection<ProfileField> Fields
        {
            get => _fields;
            set
            {
                if (_fields != null)
                {
                    _fields.CollectionChanged -= FieldsOnItemChanged;
                    _fields.ItemPropertyChanged -= FieldsOnItemChanged;
                }
                _fields = value;
                _fields.CollectionChanged += FieldsOnItemChanged;
                _fields.ItemPropertyChanged += FieldsOnItemChanged;
                OnPropertyChanged();
            }
        }

        public string SubmitButton
        {
            get => _submitButton;
            set => Set(ref _submitButton, value);
        }

        public Profile Clone()
        {
            return JsonConvert.DeserializeObject<Profile>(JsonConvert.SerializeObject(this));
        }
    }

    public class ProfileField : NotifyModel
    {
        private string _name;
        private string _value;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }

    public enum AutomationOption
    {
        Global, Autologin, Autofill, None
    }
}