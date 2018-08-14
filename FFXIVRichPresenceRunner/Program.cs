using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using DiscordRPC;
using FFXIVRichPresenceRunner.Memory;

namespace FFXIVRichPresenceRunner
{
    internal class Program
    {
        private const int SW_HIDE = 0;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void Main(string[] args)
        {
            while (!DoesFfxivProcessExist())
            {
                Console.WriteLine("Waiting for FFXIV process...");
                Thread.Sleep(200);
            }

            #if !DEBUG
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            #endif

            Run();

            while (true)
            {
            }
        }

        public static bool DoesFfxivProcessExist()
        {
            var processes = Process.GetProcesses();

            var process = processes.Where(x => x.ProcessName == "ffxiv_dx11");

            return process.ToList().Count > 0;
        }

        private static readonly RichPresence DefaultPresence = new RichPresence
        {
            Details = "Unknown",
            State = "",
            Assets = new Assets
            {
                LargeImageKey = "zone_default",
                LargeImageText = "",
                SmallImageKey = "class_0",
                SmallImageText = ""
            }
        };

        public static async void Run()
        {
            var memory = new Mem();
            if (!memory.OpenProcess("ffxiv_dx11"))
            {
                Console.WriteLine("An error occurred opening the FFXIV process.\nPress any key to continue...");

                Console.ReadKey();
                Environment.Exit(0);
            }

            var discordManager = new Discord();

            var memoryManager = new MemoryManager(memory);

            discordManager.SetPresence(DefaultPresence);

            while (true)
            {
                discordManager.Update();
                var table = memoryManager.GetActorTable();

                if (table == null)
                {
                    discordManager.SetPresence(DefaultPresence);
                    continue;
                }

                if (table.Length > 0)
                {
                    var player = table[0];

                    if(player.ActorID == 0)
                    {
                        discordManager.SetPresence(DefaultPresence);
                        continue;
                    }

                    var territoryType = memoryManager.GetTerritoryType();

                    var placename = await XivApi.GetPlaceNameZoneForTerritoryType(territoryType);
                    var zoneAsset = "zone_" + Regex.Replace(placename.ToLower(), "[^A-Za-z0-9]", "");

                    var fcName = player.CompanyTag.Substring(0, player.CompanyTag.IndexOf("\0"));

                    if (fcName != string.Empty) fcName = $" <{fcName}>";

                    discordManager.SetPresence(new RichPresence
                    {
                        Details = $"{player.Name.Substring(0, player.Name.IndexOf("\0"))}{fcName}",
                        State = await XivApi.GetNameForWorld(player.World),
                        Assets = new Assets
                        {
                            LargeImageKey = zoneAsset,
                            LargeImageText = await XivApi.GetPlaceNameForTerritoryType(territoryType),
                            SmallImageKey = $"class_{player.Job}",
                            SmallImageText = await XivApi.GetJobName(player.Job) + " Lv." + player.Level
                        }
                    });
                }
                else
                {
                    discordManager.SetPresence(DefaultPresence);
                }

                Thread.Sleep(1000);

                if (!DoesFfxivProcessExist())
                {
                    discordManager.Deinitialize();
                    Environment.Exit(0);
                }
            }
        }
    }
}