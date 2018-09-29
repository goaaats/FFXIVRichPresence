using System;
using System.Net;
using Newtonsoft.Json;

namespace FFXIVRichPresenceRunner.Memory
{
    internal class Definitions
    {
        private const string DEFINITION_JSON_URL =
            "https://goaaats.github.io/ffxiv/tools/definitions.json";

        private static Definitions _cachedInstance;

        public string ActorTableOffset = "ffxiv_dx11.exe+199DA38";
        public string TerritoryTypePtr = "ffxiv_dx11.exe+19D55E8,4C"; // 4 byte
        public string TimePtr = "ffxiv_dx11.exe+18E3330,10,8,28,80"; // 4 byte
        public string WeatherPtr = "ffxiv_dx11.exe+19579A8,27"; // 1 byte

        public int ActorIDOffset = 0x74;
        public int NameOffset = 0x30;
        public int BnpcBaseOffset = 0x80;
        public int OwnerIDOffset = 0x84;
        public int ModelCharaOffset = 0x16FC;
        public int JobOffset = 0x1788;
        public int LevelOffset = 0x178A;
        public int WorldOffset = 0x1744;
        public int CompanyTagOffset = 0x16A2;
        public int CustomizeOffset = 0x1688;
        public int RefreshSwitchOffset = 0x104;
        public int ObjectKindOffset = 0x8C;

        public int HeadOffset = 0x15E8;
        public int BodyOffset = 0x15EC;
        public int HandsOffset = 0x15F0;
        public int LegsOffset = 0x15F4;
        public int FeetOffset = 0x15F8;
        public int EarOffset = 0x15FC;
        public int NeckOffset = 0x1600;
        public int WristOffset = 0x1604;
        public int RRingOffset = 0x1608;
        public int LRingOffset = 0x160C;

        public int MainWepOffset = 0x1342;
        public int OffWepOffset = 0x13A8;

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
                        #if DEBUG
                        return new Definitions();
                        #endif

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