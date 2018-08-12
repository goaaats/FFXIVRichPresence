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

        public string ActorTableOffset = "ffxiv_dx11.exe+18FF6B8";
        public string TerritoryTypePtr = "ffxiv_dx11.exe+1936928,4C"; // 4 byte
        public string TimePtr = "ffxiv_dx11.exe+18E3330,10,8,28,80"; // 4 byte
        public string WeatherPtr = "ffxiv_dx11.exe+18E1278,27"; // 1 byte

        public int ActorIDOffset = 0x74;
        public int NameOffset = 0x30;
        public int BnpcBaseOffset = 0x80;
        public int OwnerIDOffset = 0x84;
        public int ModelCharaOffset = 0x16FC;
        public int JobOffset = 0x1738;
        public int LevelOffset = 0x173A;
        public int WorldOffset = 0x16F4;
        public int CompanyTagOffset = 0x164A;

        public string ClientID = "478143453536976896";

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