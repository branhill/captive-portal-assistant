using System;

namespace CaptivePortalAssistant.Models
{
    public class SettingItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type Page { get; set; }

        public SettingItem(string title = null, string icon = null, Type page = null)
        {
            Title = title;
            Icon = icon;
            Page = page;
        }
    }
}