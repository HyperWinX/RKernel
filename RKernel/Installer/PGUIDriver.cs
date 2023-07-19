using System;
using System.Collections.Generic;
using System.Threading;

namespace RKernel.Installer
{
    public class PGUIDriver
    {
        public PGUIDriver()
        {

        }
        public void DrawTitle()
        {
            System.Console.Write(new string(' ', 34));
            System.Console.WriteLine("AntHill OS Installer");
            System.Console.Write("\n\n\n\n\n");
        }
        public void DrawPartitionTable(List<string> partitions, int selection = 1)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.Clear();
            DrawTitle(); //title
            System.Console.Write(" " + new string('#', 88) + "\n"); //first line
            System.Console.Write(" #" + new string(' ', 86) + "# "); //just bias
            //if (partitions.Count == 0)
            if (partitions.Count == 0)
            {
                System.Console.Write(" # ");
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.Write("Create new partition");
                System.Console.Write(new string(' ', 65)); //fill
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.BackgroundColor = ConsoleColor.Blue;
                System.Console.Write("# ");
            }
            for (int i = 0; i < partitions.Count; i++)
            {
                if (i == selection - 1)
                {
                    System.Console.Write(" # ");
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    System.Console.BackgroundColor = ConsoleColor.White;
                    System.Console.Write(partitions[i] + new string(' ', 84 - partitions[i].Length));
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.Write(" #\n");
                }
                else
                    System.Console.Write(" # " + partitions[i] + new string(' ', 85 - partitions[i].Length) + "#\n");
            }
            System.Console.Write(" #" + new string(' ', 86) + "#\n");
            System.Console.Write(" " + new string('#', 88) + "\n"); //last line
        }
        public void DrawDriveTable(List<string> drives, int selection = 1)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.Clear();
            DrawTitle();
            System.Console.Write(" " + new string('#', 88) + "\n");
            System.Console.Write(" #" + new string(' ', 86) + "#\n");
            for (int i = 0; i < drives.Count; i++)
            {
                string disk = drives[i];
                System.Console.Write(" # ");
                if (selection - 1 == i)
                {
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    System.Console.BackgroundColor = ConsoleColor.White;
                }
                System.Console.Write(disk);
                System.Console.Write(new string(' ', 90 - 6 - disk.Length));
                if (selection - 1 == i)
                {
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                }
                System.Console.Write(" #\n");
            }
            System.Console.Write(" #" + new string(' ', 86) + "#\n");
            System.Console.Write(" " + new string('#', 88));
        }
        public void UpdateDiskSelection(bool increase, ref int selection, List<string> drives)
        {
            System.Console.SetCursorPosition(3, 7 + selection);
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.Write(drives[selection - 1]);
            System.Console.Write(new string(' ', 84 - drives[selection - 1].Length));
            if (increase)
            {
                selection = selection + 1;
                System.Console.SetCursorPosition(3, 7 + selection);
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.Write(drives[selection - 1]);
                System.Console.Write(new string(' ', 84 - drives[selection - 1].Length));
                System.Console.SetCursorPosition(89, 29);
            }
            else
            {
                selection = selection - 1;
                System.Console.SetCursorPosition(3, 7 + selection);
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.Write(drives[selection - 1]);
                System.Console.Write(new string(' ', 84 - drives[selection - 1].Length));
                System.Console.SetCursorPosition(89, 29);
            }
        }
        public void UpdatePartitionSelection(bool increase, ref int selection, List<string> partitions)
        {
            System.Console.SetCursorPosition(3, 7 + selection);
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.Write(partitions[selection - 1]);
            System.Console.Write(new string(' ', 84 - partitions[selection - 1].Length));
            if (increase)
            {
                selection += 1;
                System.Console.SetCursorPosition(3, 7 + selection);
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.Write(partitions[selection - 1]);
                System.Console.Write(new string(' ', 84 - partitions[selection - 1].Length));
                System.Console.SetCursorPosition(89, 29);
            }
            else
            {
                selection -= 1;
                System.Console.SetCursorPosition(3, 7 + selection);
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.Write(partitions[selection - 1]);
                System.Console.Write(new string(' ', 84 - partitions[selection - 1].Length));
                System.Console.SetCursorPosition(89, 29);
            }
        }
        public string[] DrawUserCreationMenu()
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.Clear();
            DrawTitle(); //title
            System.Console.Write(" " + new string('#', 88) + "\n"); //first line
            System.Console.Write(" #" + new string(' ', 86) + "#\n"); //just bias
            System.Console.Write(" # Select username:" + new string(' ', 69) + "#\n");
            System.Console.Write(" # > " + new string(' ', 83) + "#\n");
            System.Console.Write(" #" + new string(' ', 86) + "#\n"); //just bias
            System.Console.Write(" " + new string('#', 88) + "\n"); //first line
            System.Console.SetCursorPosition(4, 9);
            string username = "";
            username = System.Console.ReadLine();
            System.Console.Clear();
            DrawTitle(); //title
            System.Console.Write(" " + new string('#', 88) + "\n"); //first line
            System.Console.Write(" #" + new string(' ', 86) + "#\n"); //just bias
            System.Console.Write(" # Create new password:" + new string(' ', 65) + "#\n");
            System.Console.Write(" # > " + new string(' ', 83) + "#\n");
            System.Console.Write(" #" + new string(' ', 86) + "# "); //just bias
            System.Console.Write(" " + new string('#', 88) + "\n"); //first line
            System.Console.SetCursorPosition(4, 9);
            string password = "";
            password = System.Console.ReadLine();
            return new string[2] { username, password };
        }
    }
}
