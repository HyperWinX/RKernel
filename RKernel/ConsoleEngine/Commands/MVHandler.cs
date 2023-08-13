using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class MVHandler
    {
        public static void HandleMVRequest(string quer)
        {
            string[] query = quer.Split(' ');
            if (query.Length != 3 || query[0].GetHashCode() != "mv".GetHashCode())
            {
                Log.Error("Cannot handle MV request: corrupted, or incorrect request.");
                return;
            }
            if (Directory.Exists(query[1]) && Directory.Exists(query[2]))
            {
                MoveFolderRecursively(query[1], query[2]);
                return;
            }
        }
        public static void MoveFolderRecursively(string sourceFolderPath, string destinationFolderPath)
        {
            Directory.CreateDirectory(destinationFolderPath);
            string[] files = Directory.GetFiles(sourceFolderPath);
            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                File.Copy(filePath, destinationFilePath, true);
                File.Delete(filePath);
            }
            string[] folders = Directory.GetDirectories(sourceFolderPath);
            foreach (string folderPath in folders)
            {
                string folderName = Path.GetFileName(folderPath);
                string destinationSubFolderPath = Path.Combine(destinationFolderPath, folderName);
                MoveFolderRecursively(folderPath, destinationSubFolderPath);
                Directory.Delete(folderPath, true);
            }
        }
    }
}
