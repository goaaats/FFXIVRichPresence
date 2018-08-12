using System;
using System.Net;
using Newtonsoft.Json;

namespace FFXIVRichPresenceRunner.Memory
{
    internal class Definitions
    {
        private const string DEFINITION_JSON_URL =
            "https://raw.githubusercontent.com/goaaats/FFXIVRichPresence/master/definitions.json";

        private static Definitions _cachedInstance;

        public string ACTORTABLEOFFSET = "ffxiv_dx11.exe+18FF6B8";

        public string ClientID = "478143453536976896";
        public string TERRITORYTYPEOFFSETPTR = "ffxiv_dx11.exe+1936928,4C"; // 4 byte

        public string TIMEOFFSETPTR = "ffxiv_dx11.exe+18E3330,10,8,28,80"; // 4 byte
        public string WEATHEROFFSETPTR = "ffxiv_dx11.exe+18E1278,27"; // 1 byte

        public static Definitions Instance
        {
            get
            {
                if (_cachedInstance != null)
                    return _cachedInstance;

                using (var client = new WebClient())
                {
                    try
                    {
                        var result = client.DownloadString(DEFINITION_JSON_URL);
                        _cachedInstance = JsonConvert.DeserializeObject<Definitions>(result);

                        return _cachedInstance;
                    }
                    catch (Exception)
                    {
                        _cachedInstance = new Definitions();
                        return _cachedInstance;
                    }
                }
            }
        }

        public static string Json => JsonConvert.SerializeObject(new Definitions());
    }
}