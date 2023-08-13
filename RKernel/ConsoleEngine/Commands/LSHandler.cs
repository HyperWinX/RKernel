using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class LSHandler
    {
        public static void HandleLSRequest(string query)
        {
            int totalFiles = 0;
            int totalFolders = 0;
            List<string> strings = new List<string>();
            foreach (var file in Directory.GetFiles(Kernel.currentPath))
            {
                totalFiles++;
                strings.Add($"<FILE>         {file}");
            }
            foreach (var directory in Directory.GetDirectories(Kernel.currentPath))
            {
                totalFolders++;
                strings.Add($"<DIR>          {directory.Substring(Kernel.currentPath.Length - 3)}");
            }
            Console.Write("\n");
            foreach (var line in strings)
                Console.WriteLine(line);
            Console.WriteLine(new string(' ', 16 - totalFiles.ToString().Length) + totalFiles + " files");
            Console.WriteLine(new string(' ', 16 - totalFolders.ToString().Length) + totalFolders + " folders\n");
        }
    }
}
