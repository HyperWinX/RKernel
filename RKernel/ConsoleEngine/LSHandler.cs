using Cosmos.HAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class LSHandler
    {
        public LSHandler() { }
        public void HandleLSRequest()
        {
            long totalBytes = 0;
            int totalFiles = 0;
            int totalFolders = 0;
            List<string> strings = new List<string>();
            foreach (var file in Directory.GetFiles(Kernel.currentPath))
            {
                FileInfo fi = new FileInfo(file);
                totalBytes += fi.Length;
                totalFiles++;
                strings.Add($"<FILE>              {fi.Name}");
            }
            foreach (var directory in Directory.GetDirectories(Kernel.currentPath))
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                totalFolders++;
                strings.Add($"<DIRECTORY>          {di.Name}");
            }
            Console.Write("\n");
            foreach (var line in strings)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine(new string(' ', 16 - totalFiles.ToString().Length) + totalFiles + " files, " + totalBytes + " bytes");
            Console.WriteLine(new string(' ', 16 - totalFolders.ToString().Length) + totalFolders + " folders\n");
        }
    }
}
