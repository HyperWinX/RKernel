using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;

namespace RKernel.Installer
{
    public class DiskSelector
    {
        private List<string> drives;
        private int selection;
        private PGUIDriver driver;
        public DiskSelector(PGUIDriver driver)
        {
            selection = 1;
            drives = new List<string>();
            this.driver = driver;
            for (int i = 0; i < Kernel.fs.Disks.Count; i++)
                drives.Add($"Drive #{i + 1}, {Kernel.fs.Disks[i].Size / 1024 / 1024}MB, {(Kernel.fs.Disks[i].IsMBR ? "MBR" : "UNKNOWN")}");
        }
        public Disk Run()
        {
            driver.DrawDriveTable(drives, selection);
            while (!IsPressedKeyEnter(out ConsoleKey key))
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selection == 1)
                            break;
                        else
                            driver.UpdateDiskSelection(false, ref selection, drives);
                        break;
                    case ConsoleKey.DownArrow:
                        if (selection == drives.Count)
                            break;
                        else
                            driver.UpdateDiskSelection(true, ref selection, drives);
                        break;
                }
            }
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            return Kernel.fs.Disks[selection - 1];
        }
        private bool IsPressedKeyEnter(out ConsoleKey key)
        {
            key = System.Console.ReadKey().Key;
            if (key == ConsoleKey.Enter)
                return true;
            else
                return false;
        }
        public void Dispose()
        {
            drives = null;
            driver = null;
            selection = 0x00;
        }
    }
}
