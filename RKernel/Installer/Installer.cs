using Cosmos.System.FileSystem;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace RKernel.Installer
{
    public class Installer
    {
        private Disk disk;
        private ManagedPartition partition;
        private PGUIDriver driver;
        public Installer()
        {
            driver = new PGUIDriver();
        }
        public void Run()
        {
            Console.Clear();
#pragma warning disable CA1416
            Console.SetWindowSize(90, 30);
#pragma warning restore CA1416
            DiskSelector dselector = new DiskSelector(driver);
            disk = dselector.Run();
            dselector.Dispose();
            Cosmos.Core.Memory.Heap.Collect();
            PartitionSelector pselector = new PartitionSelector(disk, driver);
            partition = pselector.Run();
            pselector.Dispose();
            Cosmos.Core.Memory.Heap.Collect();
            UserConfigurator usrconf = new UserConfigurator(driver);
            string[] userdata = usrconf.Run();
            usrconf = null;
            Cosmos.Core.Memory.Heap.Collect();
            if (partition != null)
            {
                if (!RunPartitionConfiguration(disk))
                {
                    System.Console.ForegroundColor = System.ConsoleColor.White;
                    System.Console.BackgroundColor = System.ConsoleColor.Black;
                    System.Console.Clear();
                    Log.Error("Cannot automatically configure partitions!");
                    Log.Error("Please configure manually.");
                }
            }
            Console.Clear();
            Console.SetCursorPosition(0, 29);
            Console.WriteLine("Testing drive...");
            if (File.Exists("0:\\test"))
                try { File.Delete("0:\\"); } catch { }
            try { File.Create("0:\\test"); } catch { }
            if (!File.Exists("0:\\test"))
            {
                Log.Error("Partition is not accessible. Please configure partitions manually.");
                Thread.Sleep(5000);
                Cosmos.System.Power.Shutdown();
            }
            else
                File.Delete("0:\\test");
            Console.Write("Installing system... 0%");
            foreach (var directory in Directory.GetDirectories("0:\\"))
                Directory.Delete(directory, true);
            foreach (var file in Directory.GetFiles("0:\\"))
                File.Delete(file);
            Console.SetCursorPosition(Console.CursorLeft - 2, Console.CursorTop);
            Console.Write("25%");
            Directory.CreateDirectory(@"0:\RKernel");
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("50%");
            File.Create(@"0:\RKernel\currentinstall.dat").Close();
            File.WriteAllLines(@"0:\RKernel\currentinstall.dat", new string[2] { "OSname=RKernel", "Version=0.1a" });
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.Write("75%");
            File.Create("0:\\RKernel\\user.dat").Close();
            File.WriteAllLines("0:\\RKernel\\user.dat", new string[3] { $"Username={userdata[0]}", $"Password={userdata[1]}", $"RootPassword={userdata[2]}" });
            Console.SetCursorPosition(Console.CursorLeft - 3, Console.CursorTop);
            Console.WriteLine("100%");
            Console.WriteLine("Installation completed. Rebooting...");
            Thread.Sleep(3000);
            Cosmos.System.Power.Reboot();
        }
        private bool RunPartitionConfiguration(Disk drive)
        {
            try
            {
                int partitionCount = drive.Partitions.Count;
                for (int i = 1; i < partitionCount; i++)
                    drive.DeletePartition(i - 1);
                if (!drive.IsMBR)
                    return false;
                drive.CreatePartition(drive.Size - 1024);
                drive.FormatPartition(0, "FAT32", false);
                drive.MountPartition(0);
            } catch
            {
                return false;
            }
            return true;
        }
    }
}
