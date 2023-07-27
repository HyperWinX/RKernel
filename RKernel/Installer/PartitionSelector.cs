using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.Security;

namespace RKernel.Installer
{
    public class PartitionSelector
    {
        private Disk drive;
        private List<string> partitions;
        private int selection;
        private PGUIDriver driver;
        public PartitionSelector(Disk drive, PGUIDriver driver)
        {
            this.drive = drive;
            this.driver = driver;
            selection = 1;
            partitions = new List<string>();
            partitions.Add("Manage partitions automatically");
        }
        public ManagedPartition Run()
        {
            driver.DrawPartitionTable(partitions, selection);
            while (!IsPressedKeyEnter(out ConsoleKey key))
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selection == 1)
                            break;
                        else
                            driver.UpdatePartitionSelection(false, ref selection, partitions);
                        break;
                    case ConsoleKey.DownArrow:
                        if (selection == partitions.Count)
                            break;
                        else
                            driver.UpdatePartitionSelection(true, ref selection, partitions);
                        break;
                }
            }
            if (selection == partitions.Count)
                return null;
            else
                return drive.Partitions[selection - 1];
        }
        private bool IsPressedKeyEnter(out ConsoleKey key)
        {
            key = System.Console.ReadKey().Key;
            if (key == ConsoleKey.Enter)
                return true;
            else
            {
                return false;
            }
        }
        public void Dispose()
        {
            drive = null;
            partitions = null;
            selection = 0x00;
            driver = null;
        }
    }
}
