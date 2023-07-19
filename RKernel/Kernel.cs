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
        public static string OSname, Version, UserName, Passwd;
        private static TimeSpan time;
        protected override void BeforeRun()
        {
            fs = InitializeVFS();
            Console.Clear();
            if (fs.Disks.Count == 0)
            {
                Console.WriteLine("Drives not found! Cannot proceed.");
                Thread.Sleep(-1);
            }
            foreach (Disk drive in fs.Disks)
                drive.Mount();
            bool found = false;
            for (int i = 0; i < 10; i++)
                if (File.Exists($"{i}:\\RKernel\\currentinstall.dat"))
                    found = true;
            if (!found)
            {
                Console.WriteLine("No installed RKernel instances detected. Starting installer...");
                Thread.Sleep(2000);
                RKernel.Installer.Installer installer = new RKernel.Installer.Installer();
                installer.Run();
            }
            found = false;
            for (int i = 0; i < 10; i++)
                if (File.Exists($"{i}:\\RKernel\\user.dat"))
                    found = true;
            if (!found)
                RaiseCriticalKernelError("Can't find user configuration file!");
            string[] lines = File.ReadAllLines("0:\\RKernel\\currentinstall.dat");
            OSname = lines[0].Split('=')[1];
            Version = lines[1].Split('=')[1];
            lines = File.ReadAllLines("0:\\RKernel\\user.dat");
            UserName = lines[0].Split(':')[0];
            Passwd = lines[0].Split(':')[1];
            //time = bootTime.TimeOfDay.Subtract(DateTime.Now.TimeOfDay);
        }

        protected override void Run()
        {
            bool usr = false;
            bool passwd = false;
            while (!usr)
            {
                Console.Write("Enter username: ");
                string usrname = Console.ReadLine();
                if (usrname != UserName)
                {
                    Log.Error("Incorrect username!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    continue;
                }
                usr = true;
            }
            while (!passwd)
            {
                Console.Write("Enter password: ");
                string usrname = Console.ReadLine();
                if (usrname != Passwd)
                {
                    Log.Error("Incorrect password!");
                    Thread.Sleep(2000);
                    Console.Clear();
                    continue;
                }
                passwd = true;
            }
            Console.Clear();
            Console.WriteLine("Welcome to the " + OSname + " version " + Version + "\n");
            //Kernel.PrintDebug(time.ToString());
            //Console.WriteLine("Boot time: " + time.ToString());
            Engine cEngine = new Engine();
            cEngine.RunEngine();
        }
    }
}
