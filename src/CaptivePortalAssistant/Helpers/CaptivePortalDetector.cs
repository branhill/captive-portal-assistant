using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CaptivePortalAssistant.Helpers
{
    public enum DetectionMethod { Android, Windows }

    public static class CaptivePortalDetector
    {
        public static DetectionMethod DefaultDetectionMethod { get; set; } = DetectionMethod.Android;

        public static string CaptivePortalUrl { get; set; } = "http://connectivitycheck.gstatic.com/generate_204";

        public static string ActiveDnsProbeHost { get; set; } = "dns.msftncsi.com";

        public static string ActiveDnsProbeContent { get; set; } = "131.107.255.255";

        public static string ActiveWebProbeUrl { get; set; } = "http://www.msftconnecttest.com/connecttest.txt";

        public static string ActiveWebProbeContent { get; set; } = "Microsoft Connect Test";

        public static async Task<bool> IsCaptivePortalAsync()
        {
            switch (DefaultDetectionMethod)
            {
                default:
                case DetectionMethod.Android:
                    return await IsCaptivePortalAndroidAsync();
                case DetectionMethod.Windows:
                    return await IsCaptivePortalWindowsAsync();
            }
        }

        public static async Task<bool> IsCaptivePortalAndroidAsync()
        {
            var httpClient = CreateHttpClient();
            try
            {
                var response = await httpClient.GetAsync(CaptivePortalUrl);
                return response.StatusCode != HttpStatusCode.NoContent;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public static async Task<bool> IsCaptivePortalWindowsAsync()
        {
            var httpClient = CreateHttpClient();
            try
            {
                var httpResponseMessage = await httpClient.GetAsync(ActiveWebProbeUrl);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (content == ActiveWebProbeContent)
                        return false;
                }
            }
            catch (HttpRequestException)
            {
                // Ignore and keep going
            }

            try
            {
                var ipAddresses = await Dns.GetHostAddressesAsync(ActiveDnsProbeHost);
                return ipAddresses
                    .Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    .Any(ipAddress => ipAddress.ToString() == ActiveDnsProbeContent);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseProxy = false
            };
            var httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = new TimeSpan(0, 0, 10)
            };

            return httpClient;
        }
    }
}
