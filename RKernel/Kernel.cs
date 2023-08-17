using Cosmos.System.FileSystem;
using RKernel.ConsoleEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;
using RKernel.KernelFeatures;
using System.Linq;
using Cosmos.System.Graphics.Fonts;
using System.Runtime.CompilerServices;
using System.Data;
using System.Net.NetworkInformation;

namespace RKernel
{
    public class Kernel : Sys.Kernel
    {
        public static CosmosVFS fs;
        //Configurations
        public static string OSname { get => SystemConfig["OSname"]; }
        public static string Version { get => SystemConfig["Version"]; }
        public static int RunnerMemorySize { get => Convert.ToInt32(SystemConfig["RunnerMemorySize"]); }
        public static int DefaultRunnerMemorySize { get => Convert.ToInt32(SystemConfig["DefaultRunnerMemorySize"]); }
        public static string UserName { get => UserConfig["Username"]; }
        public static string Passwd { get => UserConfig["Password"]; }
        public static string RootPasswd { get => UserConfig["RootPassword"]; }
        public static bool IsDebugMode { get => Convert.ToBoolean(SystemConfig["Debug"]); }
        public static string[] usrlines;
        public static string currentPath;
        public static string currentMode;
        public static bool IsRoot;
        public static List<int> ProtectedPaths;
        public static Config UserConfig;
        public static Config SystemConfig;
        public static PowerCTL PWRController;
        protected override void BeforeRun()
        {
            try { fs = InitializeVFS(); } catch { Log.Error("Cannot register VFS!"); Console.ReadKey(); }
            PrintDebug("Disks found: " + fs.Disks.Count);
            if (fs.Disks.Count == 0)
            {
                Console.WriteLine("Drives not found! Cannot proceed.");
                PrintDebug("No drives found! Waiting for key press...");
                Console.Write("Press any key to shutdown...");
                Console.ReadKey();
                Sys.Power.Shutdown();
            }
            bool found = false;
            for (int i = 0; i < 10; i++)
                if (File.Exists($"{i}:\\RKernel\\currentinstall.dat"))
                    found = true;
            if (found)
                PrintDebug("Found installation file. Continuing kernel startup...");
            if (!found)
            {
                Console.WriteLine("No installed RKernel instances detected. Starting installer...");
                PrintDebug("Not found installation file. Starting installer...");
                Thread.Sleep(2000);
                Installer.Installer installer = new Installer.Installer();
                installer.Run();
            }
            LoadConfig(this);
        }
        protected override void Run()
        {
            bool usr = false;
            bool passwd = false;
            int tries = 0;
            while (!usr)
            {
                Console.Write("Enter username: ");
                string usrname = Console.ReadLine();
                if (usrname != UserName)
                {
                    Log.Error("Incorrect username!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    tries++;
                    continue;
                }
                usr = true;
            }
            PrintDebug("Correct username was entered after " + tries + " tries.");
            tries = 0;
            while (!passwd)
            {
                Console.Write("Enter password: ");
                string usrname = Console.ReadLine();
                if (usrname != Passwd)
                {
                    Log.Error("Incorrect password!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    tries++;
                    continue;
                }
                passwd = true;
            }
            PrintDebug("Correct password was entered after " + tries + " tries.");
            //File.Create("0:\\RKernel\\log").Close();
            //File.WriteAllLines("0:\\RKernel\\log", bootlog.ToArray());
            Console.Clear();
            Console.WriteLine("Welcome to the " + OSname + " version " + Version + "\n");
            currentPath = "0:\\";
            currentMode = "";
            IsRoot = false;
            ProtectedPaths = new List<int>
            {
                @"0:\RKernel".GetHashCode(),
                @"0:\RKernel\currentinstall.dat".GetHashCode(),
                @"0:\RKernel\user.dat".GetHashCode()
            };
            //Kernel.PrintDebug(time.ToString());
            //Console.WriteLine("Boot time: " + time.ToString());
            Engine cEngine = new Engine();
            cEngine.RegisterCommands();
            cEngine.RunEngine();
        }
        private static void LoadConfig(Kernel kernel)
        {
            Console.Write("Loading config...           ");
            try
            {
                bool found = false;
                for (int i = 0; i < 10; i++)
                    if (File.Exists($"{i}:\\RKernel\\user.dat"))
                        found = true;
                if (!found)
                {
                    PrintDebug("Not found user configuration file! Shutting down...");
                    File.Create("0:\\RKernel\\currentinstall.dat").Close();
                    File.WriteAllLines("0:\\RKernel\\currentinstall.dat", null);
                    File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[1] { "corrupted=1" });
                    kernel.RaiseCriticalKernelError("Can't find user configuration file!");
                }
                PrintDebug("Loading system config");
                SystemConfig = new("0:\\RKernel\\currentinstall.dat");
                if (SystemConfig.GetConfigEntries().Contains("corrupted"))
                {
                    Log.Warning("Previous installation was corrupted! Starting installer...");
                    Thread.Sleep(2000);
                    Installer.Installer installer = new Installer.Installer();
                    installer.Run();
                }
                PrintDebug("Current OS name: " + OSname);
                PrintDebug("Current OS version: " + Version);
                PrintDebug("Loading user data");
                UserConfig = new("0:\\RKernel\\user.dat");
                if (UserConfig.GetConfigEntries().Length != 3)
                {
                    File.WriteAllLines("0:\\RKernel\\user.dat", null);
                    File.WriteAllLines("0:\\RKernel\\user.dat", new string[1] { "corrupted=1" });
                    kernel.RaiseCriticalKernelError("Corrupted user configuration file!");
                }
                Log.Success("[OK]");
            }
            catch
            {
                Log.Error("[FAILURE]");
            }
        }
    }
}
