using System;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace CaptivePortalAssistant.Helpers
{
    public static class WifiInfo
    {
        public static async Task<string> GetSsid()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null && profile.IsWlanConnectionProfile)
            {
                return profile.WlanConnectionProfileDetails.GetConnectedSsid();
            }

            var access = await WiFiAdapter.RequestAccessAsync();
            if (access == WiFiAccessStatus.Allowed)
            {
                var adapters = await WiFiAdapter.FindAllAdaptersAsync();
                foreach (var adapter in adapters)
                {
                    var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
                    if (connectedProfile != null)
                    {
                        return connectedProfile.ProfileName;
                    }
                }
            }
            return string.Empty;
        }
    }
}
