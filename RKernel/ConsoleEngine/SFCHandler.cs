using Cosmos.Core;
using Cosmos.Core.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    internal class SFCHandler
    {
        List<int> errorIDs;
        public SFCHandler() { errorIDs = new List<int>(); }
        public List<int> HandleSFCRequest(string[] query)
        {
            if (query[0] != "sfc")
            {
                Log.Error("Cannot handle SFC request: corrupted or incorrect request.");
                return errorIDs;
            }
            switch (query[1])
            {
                case "help":
                    Console.Write("\nUsage:\n");
                    Console.WriteLine("sfc -verify - initialize file integrity check");
                    Console.WriteLine("sfc -repair - initialize restoration process\n");
                    break;
                case "verify":
                    bool userFileExist = true;
                    bool installFileExist = true;
                    Log.Warning("Starting SFC in verify mode...");
                    Log.Warning("Scanning file system...");
                    if (Kernel.fs.Disks[0].Partitions.Count == 0)
                    {
                        Log.Error("No partitions on main drive detected!");
                        Log.Warning("Cannot proceed: file system corrupted. Please recover.");
                        errorIDs.Add(ErrorIDs.ids["IncorrectRequest"]);
                        return errorIDs;
                    }
                    else if (Kernel.fs.Disks[0].Partitions.Count > 1)
                    {
                        Log.Warning("Incorrect partition count!");
                        errorIDs.Add(ErrorIDs.ids["IncorrectPartitionCount"]);
                    }
                    if (!Directory.Exists(@"0:\RKernel"))
                    {
                        errorIDs.Add(ErrorIDs.ids["NoRootDirectory"]);
                        Log.Error("Not existing system directory, scanning stopped.");
                        return errorIDs;
                    }
                    if (!File.Exists(@"0:\RKernel\currentinstall.dat"))
                    {
                        errorIDs.Add(ErrorIDs.ids["NoInstallationFile"]);
                        Log.Error("Not existing installation file.");
                        installFileExist = false;
                    }
                    if (!File.Exists("0:\\RKernel\\user.dat"))
                    {
                        errorIDs.Add(ErrorIDs.ids["NoUserFile"]);
                        Log.Error("No user file.");
                        userFileExist = false;
                    }
                    if (installFileExist)
                    {
                        string[] lines = File.ReadAllLines(@"0:\RKernel\currentinstall.dat");
                        bool same = true;
                        if (lines[0] != "OSname=RKernel")
                            same = false;
                        if (lines[1] != "Version=0.1a")
                            same = false;
                        if (!same)
                        {
                            errorIDs.Add(ErrorIDs.ids["IncorrectInstallationData"]);
                            Log.Error("Incorrect data in installation file.");
                        }
                    }
                    if (userFileExist)
                    {
                        string[] lines = File.ReadAllLines("0:\\RKernel\\user.dat");
                        if (lines.Length != 1)
                        {
                            errorIDs.Add(ErrorIDs.ids["CorruptedUserData"]);
                            Log.Error("Corrupted data in user file.");
                            return errorIDs;
                        }
                        if (lines[0].Split(':')[0] != Kernel.UserName)
                        {
                            errorIDs.Add(ErrorIDs.ids["CorruptedUserData"]);
                            Log.Error("Corrupted data in user file.");
                            return errorIDs;
                        }
                        if (lines[0].Split(':')[1] != Kernel.Passwd)
                        {
                            errorIDs.Add(ErrorIDs.ids["CorruptedUserData"]);
                            Log.Error("Corrupted data in user file.");
                            return errorIDs;
                        }
                    }
                    return errorIDs;
                case "repair":
                    Log.Warning("Running verification...");
                    SFCHandler sfchandler = new SFCHandler();
                    List<int> errors = sfchandler.HandleSFCRequest(new string[2] { "sfc", "verify" });
                    if (errors.Count == 0)
                    {
                        Log.Success("No repair needed!");
                        return null;
                    }
                    sfchandler = null;
                    switch (errors[0])
                    {
                        case 1188118005:
                            Log.Error("Bro, we cannot fix incorrect request, sorry:(");
                            return null;
                        case 2116877334:
                            Log.Error("NoDrive error is software unfixable.");
                            return null;
                        case 711894109:
                            Log.Warning("Started recovering incorrect partition count...");
                            Log.Warning("Removing partitions...");
                            for (int i = 1; i < Kernel.fs.Disks[0].Partitions.Count + 1; i++)
                                Kernel.fs.Disks[0].DeletePartition(i);
                            Log.Warning("Creating partition...");
                            Kernel.fs.Disks[0].CreatePartition(Kernel.fs.Disks[0].Size / 1024 / 1024 - 1024);
                            Log.Warning("Formatting partition...");
                            Kernel.fs.Disks[0].FormatPartition(0, "FAT32", false);
                            Log.Warning("Mounting...");
                            Kernel.fs.Disks[0].MountPartition(0);
                            Log.Warning("Recovering system files...");
                            Directory.CreateDirectory("0:\\RKernel");
                            File.Create("0:\\RKernel\\currentinstall.dat");
                            File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[2] {"OSname=RKernel", "Version=0.1a"});
                            Log.Warning("Recovering completed, please restart system.");
                            return null;
                        case 2078965517:
                            Log.Warning("Started recovering NoRootDirectory...");
                            Directory.CreateDirectory("0:\\RKernel");
                            File.Create("0:\\RKernel\\currentinstall.dat");
                            File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[2] { "OSname=RKernel", "Version=0.1a" });
                            Log.Warning("Recovering completed, please restart your system.");
                            return null;
                        case 1922124426:
                            Log.Warning("Started recovering NoInstallationFile...");
                            File.Create("0:\\RKernel\\currentinstall.dat");
                            File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[2] { "OSname=RKernel", "Version=0.1a" });
                            Log.Warning("Recovering completed, please restart your system.");
                            return null;
                        case 882358732:
                            Log.Warning("Started recovering IncorrectInstallationData...");
                            File.Delete("0:\\RKernel\\currentinstall.dat");
                            File.Create("0:\\RKernel\\currentinstall.dat");
                            File.WriteAllLines("0:\\RKernel\\currentinstall.dat", new string[2] { "OSname=RKernel", "Version=0.1a" });
                            return null;
                        case 912971925:
                            Log.Warning("Started recovering CorruptedUserData...");
                            File.WriteAllLines("0:\\RKernel\\user.dat", null);
                            File.WriteAllLines("0:\\RKernel\\usr.dat", new string[1] { $"{Kernel.UserName}:{Kernel.Passwd}" });
                            return null;
                        case 1097540862:
                            Log.Warning("Started recovering NoUserFile...");
                            File.Create("0:\\RKernel\\user.dat").Close();
                            string usr = Kernel.UserName;
                            string passwd = Kernel.Passwd;
                            File.WriteAllLines("0:\\RKernel\\usr.dat", Kernel.usrlines);
                            return null;
                    }
                    break;
            }
            return null;
        }
    }
    static class ErrorIDs
    {
        public static Dictionary<string, int> ids = new Dictionary<string, int>
        {
            {"IncorrectRequest", 1188118005},
            {"NoDrive", 2116877334},
            {"IncorrectPartitionCount", 711894109},
            {"NoRootDirectory", 2078965517},
            {"NoInstallationFile", 1922124426},
            {"IncorrectInstallationData", 882358732},
            {"NoUserFile", 1097540862},
            {"CorruptedUserData", 912971925}
        };
        public static Dictionary<int, string> Ids = new Dictionary<int, string>
        {
            {1188118005, "IncorrectRequest"},
            {2116877334, "NoDrive"},
            {711894109, "IncorrectPartitionCount"},
            {2078965517, "NoRootDirectory"},
            {1922124426, "NoInstallationFile"},
            {882358732, "IncorrectInstallationData"},
            {912971925, "CorruptedUserData" },
            {1097540862, "NoUserFile" }
        };
    }
}
