using System;
using DiscordRPC;
using DiscordRPC.Logging;

namespace FFXIVRichPresenceRunner
{
    internal class Discord
    {
        public DiscordRpcClient _rpcClient;

        public Discord(RichPresence initialPresence, string clientId)
        {
            _rpcClient = new DiscordRpcClient(clientId, true);

            //Set the logger
            _rpcClient.Logger = new ConsoleLogger {Level = LogLevel.Warning};

            //Subscribe to events
            _rpcClient.OnPresenceUpdate += (sender, e) => { Console.WriteLine("Received Update! {0}", e.Presence); };

            //Connect to the RPC
            _rpcClient.Initialize();

            _rpcClient.SetPresence(initialPresence);
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
            if (presence.State != _rpcClient.CurrentPresence.State ||
                presence.Details != _rpcClient.CurrentPresence.Details ||
                presence.Assets.SmallImageText != _rpcClient.CurrentPresence.Assets.SmallImageText ||
                presence.Assets.LargeImageText != _rpcClient.CurrentPresence.Assets.LargeImageText)
                _rpcClient.SetPresence(presence);
        }
    }
}