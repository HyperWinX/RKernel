using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class PMHandler
    {
        public static void HandlePMRequest(string quer)
        {
            string[] query = quer.Split(" -", StringSplitOptions.None);
            if (query[0] != "pm")
            {
                Log.Error("Cannot handle PM request: corrupted, or incorrect request.");
                return;
            }
            switch (query[1])
            {
                case "help":
                    Console.Write("\nUsage:\n");
                    Console.WriteLine("pm -create -drive:0 -size:500 - Create partition on drive 0 with size of 500 mb");
                    Console.WriteLine("pm -delete -drive:0 -partition:0 - Delete partition 0 on drive 0");
                    Console.WriteLine("pm -list -drives - Get drives list");
                    Console.WriteLine("pm -list -drive:0 - Get list of partitions on drive 0");
                    break;
                case "create":
                    if (query.Length != 4)
                    {
                        Log.Error("Cannot create partition: incorrect command usage.");
                        return;
                    }
                    bool hasDriveArgument = false;
                    bool hasSizeArgument = false;
                    int indexOfDriveArg = 0;
                    int indexOfSizeArg = 0;
                    int drive, size;
                    string[] splittedDriveArgument, splittedSizeArgument;
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("drive"))
                        {
                            hasDriveArgument = true;
                            indexOfDriveArg = i;
                            i = query.Length;
                        }
                    }
                    if (!hasDriveArgument)
                    {
                        Log.Error("Cannot create partition: no \"drive\" argument.");
                        return;
                    }
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("size"))
                        {
                            hasSizeArgument = true;
                            indexOfSizeArg = i;
                            i = query.Length;
                        }
                    }
                    if (!hasSizeArgument)
                    {
                        Log.Error("Cannot create partition: no \"size\" argument.");
                        return;
                    }
                    splittedDriveArgument = query[indexOfDriveArg].Split(':');
                    if (splittedDriveArgument.Length != 2)
                    {
                        Log.Error("Cannot create partition: incorrect usage of \"-drive\" argument.");
                        return;
                    }
                    splittedSizeArgument = query[indexOfSizeArg].Split(':');
                    if (splittedDriveArgument.Length != 2)
                    {
                        Log.Error("Cannot create partition: incorrect usage of \"-size\" argument.");
                        return;
                    }
                    if (!Tools.Tools.TryParse(splittedDriveArgument[1], out drive))
                    {
                        Log.Error("Cannot create partition: not a drive number.");
                        return;
                    }
                    if (!Tools.Tools.TryParse(splittedSizeArgument[1], out size))
                    {
                        Log.Error("Cannot create partition: not a size number.");
                        return;
                    }
                    if (drive < 0 || drive > Kernel.fs.Disks.Count)
                    {
                        Log.Error("Cannot create partition: not valid drive number.");
                        return;
                    }
                    if (size < 0 || size > Kernel.fs.Disks[drive].Size / 1024 / 1024 - 1024)
                    {
                        Log.Error("Cannot create partition: cannot create partition with size bigger or equal to drive size.");
                        return;
                    }
                    try
                    {
                        Kernel.fs.Disks[drive].CreatePartition(size);
                        splittedDriveArgument = null;
                        splittedSizeArgument = null;
                        query = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unknown error while creating partition.\n" + ex.Message);
                    }
                    break;
                case "delete":
                    if (query.Length != 4)
                    {
                        Log.Error("Cannot delete partition: incorrect command usage.");
                        return;
                    }
                    bool hasDriveArgument1 = false;
                    bool hasPartitionArgument = false;
                    int indexOfDriveArg1 = 0;
                    int indexOfPartitionArg = 0;
                    int drive1, partition;
                    string[] splittedDriveArgument1, splittedPartitionArgument;
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("drive"))
                        {
                            hasDriveArgument1 = true;
                            indexOfDriveArg1 = i;
                            i = query.Length;
                        }
                    }
                    if (!hasDriveArgument1)
                    {
                        Log.Error("Cannot delete partition: no \"drive\" argument.");
                        return;
                    }
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("partition"))
                        {
                            hasPartitionArgument = true;
                            indexOfPartitionArg = i;
                            i = query.Length;
                        }
                    }
                    if (!hasPartitionArgument)
                    {
                        Log.Error("Cannot delete partition: no \"partition\" argument.");
                        return;
                    }
                    splittedDriveArgument1 = query[indexOfDriveArg1].Split(':');
                    if (splittedDriveArgument1.Length != 2)
                    {
                        Log.Error("Cannot delete partition: incorrect usage of \"drive\" argument.");
                        return;
                    }
                    splittedPartitionArgument = query[indexOfPartitionArg].Split(':');
                    if (splittedPartitionArgument.Length != 2)
                    {
                        Log.Error("Cannot delete partition: incorrect usage of \"partition\" argument.");
                        return;
                    }
                    if (!Tools.Tools.TryParse(splittedDriveArgument1[1], out drive1))
                    {
                        Log.Error("Cannot delete partition: not a drive number.");
                        return;
                    }
                    if (!Tools.Tools.TryParse(splittedPartitionArgument[1], out partition))
                    {
                        Log.Error("Cannot delete partition: not a partition number.");
                        return;
                    }
                    if (drive1 < 0 || drive1 > Kernel.fs.Disks.Count)
                    {
                        Log.Error("Cannot delete partition: not valid drive number.");
                        return;
                    }
                    if (partition < 0 || partition > Kernel.fs.Disks[drive1].Partitions.Count)
                    {
                        Log.Error("Cannot delete partition: cannot create partition with size bigger or equal to drive size.");
                        return;
                    }
                    try
                    {
                        Kernel.fs.Disks[drive1].DeletePartition(partition);
                        splittedDriveArgument1 = null;
                        splittedPartitionArgument = null;
                        query = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unknown error while creating partition.\n" + ex.Message);
                    }
                    break;
                case "list":
                    int indexOfDriveArg2 = 0;
                    bool hasDriveArgument2 = false;
                    bool hasDrivesArgument = false;
                    string[] splittedDriveArgument2;
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("drives"))
                        {
                            hasDrivesArgument = true;
                            i = query.Length;
                        }
                    }
                    if (hasDrivesArgument)
                    {
                        Console.Write("\n");
                        for (int i = 0; i < Kernel.fs.Disks.Count; i++)
                        {
                            Console.WriteLine("Drive number: " + (i + 1));
                            Console.WriteLine("Size: " + Kernel.fs.Disks[i].Size / 1024 / 1024 + "MB");
                            Console.WriteLine("IsMBR: " + Kernel.fs.Disks[i].IsMBR);
                            Console.WriteLine("Partitions count: " + Kernel.fs.Disks[i].Partitions.Count);
                        }
                        Console.Write("\n");
                        return;
                    }
                    for (int i = 1; i < query.Length; i++)
                    {
                        if (query[i].Contains("drive"))
                        {
                            hasDriveArgument2 = true;
                            indexOfDriveArg2 = i;
                            i = query.Length;
                        }
                    }
                    if (!hasDriveArgument2)
                    {
                        Log.Error("Cannot list partitions: no \"drive\" argument.");
                        return;
                    }
                    splittedDriveArgument2 = query[indexOfDriveArg2].Split(':');
                    if (splittedDriveArgument2.Length != 2)
                    {
                        Log.Error("Cannot list partitions: incorrect usage of \"drive\" argument.");
                        return;
                    }
                    if (string.IsNullOrEmpty(splittedDriveArgument2[1]) ||
                        string.IsNullOrWhiteSpace(splittedDriveArgument2[1]))
                    {
                        Log.Error("Cannot list partitions: incorrect usage of \"drive\" argument.");
                        return;
                    }
                    int drivenum;
                    if (!Tools.Tools.TryParse(splittedDriveArgument2[1], out drivenum))
                    {
                        Log.Error("Cannot list partitions: not a drive number.");
                        return;
                    }
                    if (!Tools.Tools.IsNumberInRange(drivenum, 0, Kernel.fs.Disks.Count))
                    {
                        Log.Error("Cannot list partitions: cannot find drive with such number.");
                        return;
                    }
                    try
                    {
                        Console.Write("\n");
                        for (int i = 0; i < Kernel.fs.Disks.Count; i++)
                        {
                            Console.WriteLine("Disk number: #" + i);
                            Console.WriteLine("Block count: " + Kernel.fs.Disks[i].Host.BlockCount);
                            Console.WriteLine("Block size: " + Kernel.fs.Disks[i].Host.BlockSize);
                            Console.WriteLine("Size: " + Kernel.fs.Disks[i].Size / 1024 / 1024 + "MB");
                            Console.WriteLine("IsMBR: " + Kernel.fs.Disks[i].IsMBR);
                        }
                        Console.Write("\n");
                        return;
                    }
                    catch { }
                    break;
            }
        }
    }
}
