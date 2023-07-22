using Cosmos.System.FileSystem;
using RKernel.ConsoleEngine;
using RKernel.Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;

namespace RKernel
{
    public class Kernel : Sys.Kernel
    {
        public static Cosmos.System.FileSystem.CosmosVFS fs;
        public static string OSname, Version, UserName, Passwd, RootPasswd;
        private static TimeSpan time;
        List<string> bootlog;
        public static string[] usrlines;
        public static string currentPath;
        public static string currentMode;
        protected override void BeforeRun()
        {
            try { fs = InitializeVFS(); bootlog.Add("VFS initialized"); } catch (Exception ex) { bootlog.Add("Cannot initialize VFS, error: " + ex.Message); bootlog.Add("Shutting down"); }
            Console.Clear();
            bootlog.Add("Disks found: " + fs.Disks.Count);
            if (fs.Disks.Count == 0)
            {
                Console.WriteLine("Drives not found! Cannot proceed.");
                bootlog.Add("No drives found! Waiting for key press...");
                Console.Write("Press any key to shutdown...");
                Console.ReadKey();
                Sys.Power.Shutdown();
            }
            foreach (Disk drive in fs.Disks)
                drive.Mount();
            bool found = false;
            for (int i = 0; i < 10; i++)
                if (File.Exists($"{i}:\\RKernel\\currentinstall.dat"))
                    found = true;
            if (found)
                bootlog.Add("Found installation file. Continuing kernel startup...");
            if (!found)
            {
                Console.WriteLine("No installed RKernel instances detected. Starting installer...");
                bootlog.Add("Not found installation file. Starting installer...");
                Thread.Sleep(2000);
                RKernel.Installer.Installer installer = new RKernel.Installer.Installer();
                installer.Run();
            }
            found = false;
            for (int i = 0; i < 10; i++)
                if (File.Exists($"{i}:\\RKernel\\user.dat"))
                    found = true;
            if (!found)
            {
                bootlog.Add("Not found user configuration file! Shutting down...");
                File.WriteAllLines("0:\\RKernel\\currentinstall.dat", null);
                File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[1] { "corrupted" });
                RaiseCriticalKernelError("Can't find user configuration file!");
            }
            string[] lines = File.ReadAllLines("0:\\RKernel\\currentinstall.dat");
            if (lines.Length == 1 && lines[0] == "corrupted")
            {
                Log.Warning("Previous installation was corrupted! Starting installer...");
                Thread.Sleep(2000);
                Installer.Installer installer = new Installer.Installer();
                installer.Run();
            }
            OSname = lines[0].Split('=')[1];
            Version = lines[1].Split('=')[1];
            bootlog.Add("Current OS name: " + OSname);
            bootlog.Add("Current OS version: " + Version);
            lines = File.ReadAllLines("0:\\RKernel\\user.dat");
            if (lines.Length != 3)
            {
                File.WriteAllLines("0:\\RKernel\\currentinstall.dat", null);
                File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[1] { "corrupted" });
                RaiseCriticalKernelError("Corrupted user configuration file!");
            }
            UserName = lines[0].Split('=')[1];
            Passwd = lines[1].Split('=')[1];
            RootPasswd = lines[2].Split('=')[1];
            usrlines = lines;
            //time = bootTime.TimeOfDay.Subtract(DateTime.Now.TimeOfDay);
        }

        protected override void Run()
        {
            bool usr = false;
            bool passwd = false;
            int its = 1;
            while (!usr)
            {
                Console.Write("Enter username: ");
                string usrname = Console.ReadLine();
                if (usrname != UserName)
                {
                    Log.Error("Incorrect username!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    its++;
                    continue;
                }
                usr = true;
            }
            bootlog.Add("Correct username was entered after " + its + " tries.");
            its = 0;
            while (!passwd)
            {
                Console.Write("Enter password: ");
                string usrname = Console.ReadLine();
                if (usrname != Passwd)
                {
                    Log.Error("Incorrect password!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    its++;
                    continue;
                }
                passwd = true;
            }
            bootlog.Add("Correct password was entered after " + its + " tries.");
            //File.Create("0:\\RKernel\\log").Close();
            //File.WriteAllLines("0:\\RKernel\\log", bootlog.ToArray());
            bootlog.Clear();
            bootlog = null;
            Console.Clear();
            Console.WriteLine("Welcome to the " + OSname + " version " + Version + "\n");
            currentPath = "0:\\";
            currentMode = "";
            //Kernel.PrintDebug(time.ToString());
            //Console.WriteLine("Boot time: " + time.ToString());
            Engine cEngine = new Engine();
            cEngine.RunEngine();
        }
    }
}
