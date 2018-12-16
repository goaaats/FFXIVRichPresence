using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using DiscordRPC;
using Newtonsoft.Json;

namespace FFXIVRichPresenceRunner
{
    internal class Program
    {
        private const string ClientID = "478143453536976896";

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

            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs eventArgs)
            {
                File.WriteAllText("RichPresenceException.txt", eventArgs.ExceptionObject.ToString());
                    
                Process.GetCurrentProcess().Kill();
            };

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
            var procList = Process.GetProcessesByName( "ffxiv_dx11" );

            if (procList.Length == 0)
            {
                Console.WriteLine("An error occurred opening the FFXIV process: Not found.\nPress any key to continue...");

                Console.ReadKey();
                Environment.Exit(0);
            }

            var discordManager = new Discord(DefaultPresence, ClientID);

            var game = new Nhaama.FFXIV.Game(procList[0]);

            Console.WriteLine(game.Process.GetSerializer().SerializeObject(game.Definitions, Formatting.Indented));

            discordManager.SetPresence(DefaultPresence);

            while (true)
            {
                if (!DoesFfxivProcessExist())
                {
                    discordManager.Deinitialize();
                    Environment.Exit(0);
                }

                game.Update();

                if (game.ActorTable == null)
                {
                    discordManager.SetPresence(DefaultPresence);
                    continue;
                }

                if (game.ActorTable.Length > 0)
                {
                    var player = game.ActorTable[0];

                    if(player.ActorID == 0)
                    {
                        discordManager.SetPresence(DefaultPresence);
                        continue;
                    }

                    var territoryType = game.TerritoryType;

                    var placename = await XivApi.GetPlaceNameZoneForTerritoryType(territoryType);
                    var zoneAsset = "zone_" + Regex.Replace(placename.ToLower(), "[^A-Za-z0-9]", "");

                    var fcName = player.CompanyTag;

                    if (fcName != string.Empty)
                    {
                        fcName = $" <{fcName}>";
                    }

                    discordManager.SetPresence(new RichPresence
                    {
                        Details = $"{player.Name}{fcName}",
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
                

                Thread.Sleep(1000);
            }
        }
    }
}