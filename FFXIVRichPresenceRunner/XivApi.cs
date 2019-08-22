using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FFXIVRichPresenceRunner.Properties;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FFXIVRichPresenceRunner
{
    public static class XivApi
    {
        private const string URL = "http://xivapi.com/";
        private const string Key = "1ef6047fe34b4b7a927f694e";

        private static readonly Dictionary<int, JObject> _cachedTerritoryType = new Dictionary<int, JObject>();
        private static readonly Dictionary<int, string> _cachedJobNames = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> _cachedWorldNames = new Dictionary<int, string>();

        public partial class World
        {
            [JsonProperty("Index")]
            public long Index { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public static async Task<string> GetNameForWorld(int world)
        {
            if (_cachedWorldNames.ContainsKey(world))
                return _cachedWorldNames[world];

            var res = await Get("World/" + world);
            _cachedWorldNames.Add(world, (string) res.Name);

            return res.Name;
        }

        public static async Task<int> GetLoadingImageKeyForTerritoryType(int territorytype)
        {
            if (_cachedTerritoryType.ContainsKey(territorytype))
            {
                dynamic cachedRes = _cachedTerritoryType[territorytype];
                return (int) cachedRes.LoadingImageTargetID;
            }

            var res = await Get("TerritoryType/" + territorytype);
            try
            {
                _cachedTerritoryType.Add(territorytype, res);

                return (int) res.LoadingImageTargetID;
            }
            catch (RuntimeBinderException)
            {
                _cachedTerritoryType.Add(territorytype, res);

                return 1;
            }
        }

        public static async Task<string> GetPlaceNameZoneForTerritoryType(int territorytype)
        {
            if (_cachedTerritoryType.ContainsKey(territorytype))
            {
                dynamic cachedRes = _cachedTerritoryType[territorytype];
                return (string) cachedRes.PlaceNameZone.Name_en;
            }

            var res = await Get("TerritoryType/" + territorytype);
            try
            {
                _cachedTerritoryType.Add(territorytype, res);

                return (string) res.PlaceNameZone.Name_en;
            }
            catch (RuntimeBinderException)
            {
                _cachedTerritoryType.Add(territorytype, res);

                return "Not Found";
            }
        }

        public static async Task<string> GetPlaceNameForTerritoryType(int territorytype)
        {
            if (_cachedTerritoryType.ContainsKey(territorytype))
            {
                dynamic cachedRes = _cachedTerritoryType[territorytype];
                return (string) cachedRes.PlaceName.Name_en;
            }

            var res = await Get("TerritoryType/" + territorytype);
            try
            {
                _cachedTerritoryType.Add(territorytype, res);

                return (string) res.PlaceName.Name_en;
            }
            catch (RuntimeBinderException)
            {
                _cachedTerritoryType.Add(territorytype, res);

                return "Not Found";
            }
        }

        public static async Task<string> GetJobName(int jobId)
        {
            if (_cachedJobNames.ContainsKey(jobId))
                return _cachedJobNames[jobId];

            var res = await Get("ClassJob/" + jobId);
            _cachedJobNames.Add(jobId, (string) res.NameEnglish_en);

            return res.NameEnglish_en;
        }

        public static async Task<dynamic> Get(string endpoint, params string[] parameters)
        {
            var requestParameters = "?";

            foreach (var parameter in parameters) requestParameters += parameter + "&";

            var client = new HttpClient();
            var response = await client.PostAsync(URL + endpoint + "?key=" + Key, new StringContent(requestParameters));
            var result = await response.Content.ReadAsStringAsync();

            return JObject.Parse(result);
        }
    }
}