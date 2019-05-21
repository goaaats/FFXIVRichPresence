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

        private static readonly Dictionary<int, string> _cachedTerritoryTypeZoneNames = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> _cachedTerritoryTypeNames = new Dictionary<int, string>();
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

        public static async Task<string> GetPlaceNameZoneForTerritoryType(int territorytype)
        {
            if (_cachedTerritoryTypeZoneNames.ContainsKey(territorytype))
                return _cachedTerritoryTypeZoneNames[territorytype];

            var res = await Get("TerritoryType/" + territorytype);
            try
            {
                _cachedTerritoryTypeZoneNames.Add(territorytype, (string) res.PlaceNameZone.Name_en);

                return res.PlaceNameZone.Name_en;
            }
            catch (RuntimeBinderException)
            {
                _cachedTerritoryTypeZoneNames.Add(territorytype, "default");

                return "default";
            }
        }

        public static async Task<string> GetPlaceNameForTerritoryType(int territorytype)
        {
            if (_cachedTerritoryTypeNames.ContainsKey(territorytype))
                return _cachedTerritoryTypeNames[territorytype];

            var res = await Get("TerritoryType/" + territorytype);

            try
            {
                _cachedTerritoryTypeNames.Add(territorytype, (string) res.PlaceName.Name_en);

                return res.PlaceName.Name_en;
            }
            catch
            {
                _cachedTerritoryTypeNames.Add(territorytype, "default");

                return "default";
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