using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using FFXIVRichPresenceRunner.Memory;

namespace FFXIVRichPresenceRunner
{
    class Discord
    {
        public DiscordRPC.DiscordRpcClient _rpcClient;

        private readonly RichPresence DefaultPresence = new RichPresence()
        {
            Details = "Unknown",
            State = "",
            Assets = new Assets()
            {
                LargeImageKey = "eorzea_map",
                LargeImageText = "",
                SmallImageKey = "class_0",
                SmallImageText = ""
            }
        };	

        public Discord()
        {
            Initialize();
        }

        public void Update() 
        {
            //Invoke all the events, such as OnPresenceUpdate
            _rpcClient.Invoke();
        }

        public void Deinitialize() 
        {
            _rpcClient.Dispose();
        }

        public void SetPresence(RichPresence presence)
        {
            if(presence.State != _rpcClient.CurrentPresence.State || presence.Details != _rpcClient.CurrentPresence.Details || presence.Assets.SmallImageText != _rpcClient.CurrentPresence.Assets.SmallImageText || presence.Assets.LargeImageText != _rpcClient.CurrentPresence.Assets.LargeImageText)
                _rpcClient.SetPresence(presence);
        }

        public void SetDefaultPresence()
        {
            SetPresence(DefaultPresence);
        }

        void Initialize() 
        {
            /*
            Create a discord client
            NOTE: 	If you are using Unity3D, you must use the full constructor and define
                     the pipe connection as DiscordRPC.IO.NativeNamedPipeClient
            */
            _rpcClient = new DiscordRpcClient(Definitions.Instance.ClientID, true);					
	
            //Set the logger
            _rpcClient.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            _rpcClient.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };
	
            //Connect to the RPC
            _rpcClient.Initialize();

            //Set the rich presence
            _rpcClient.SetPresence(DefaultPresence);
        }
    }
}
