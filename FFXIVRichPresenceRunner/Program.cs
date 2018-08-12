using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using FFXIVPlayerWardrobe.Memory;
using FFXIVRichPresenceRunner.Memory;

namespace FFXIVRichPresenceRunner
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            while (!DoesFfxivProcessExist())
            {
                Console.WriteLine("Waiting for FFXIV process...");
                System.Threading.Thread.Sleep(200);
            }

            /*
            if (args.Length > 0)
                ShowWindow(GetConsoleWindow(), SW_HIDE);
                */

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

        public static async void Run()
        {
            Mem memory = new Mem();
            if (!memory.OpenProcess("ffxiv_dx11"))
            {
                Console.WriteLine("An error occurred opening the FFXIV process.\nPress any key to continue...");

                Console.ReadKey();
                Environment.Exit(0);
            }

            Discord discordManager = new Discord();

            MemoryManager memoryManager = new MemoryManager(memory);

            discordManager.SetDefaultPresence();

            while (true)
            {
                discordManager.Update();
                var table = memoryManager.GetActorTable();

                if (table.Length > 0)
                {
                    var player = table[0];

                    var territoryType = memoryManager.GetTerritoryType();

                    var placename = await XivApi.GetPlaceNameZoneForTerritoryType(territoryType);
                    var zoneAsset = "zone_" + Regex.Replace(placename.ToLower(), "[^A-Za-z0-9]","");

                    discordManager.SetPresence(new RichPresence()
                    {
                        Details = $"{player.Name.Substring(0, player.Name.IndexOf("\0"))}",
                        State = "Idle",
                        Assets = new Assets()
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
                    discordManager.SetDefaultPresence();
                }

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
