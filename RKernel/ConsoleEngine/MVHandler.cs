using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class MVHandler
    {
        public MVHandler() { }
        public void HandleMVRequest(string[] query)
        {
            if (query.Length != 3 || query[0].GetHashCode() != "mv".GetHashCode())
            {
                Log.Error("Cannot handle MV request: corrupted, or incorrect request.");
                return;
            }
            if (!File.Exists(query[1]) ||
                !Directory.Exists(query[1]) ||
                !File.Exists(query[2]) ||
                !Directory.Exists(query[2]))
            {
                Log.Error("One of targets does not exist!");
                return;
            }
            if (File.Exists(query[1]))
            {
                if (File.Exists(query[2]))
                    FileMove(query[1], query[2]);
                else if (Directory.Exists(query[2]))
                    FileMove(query[1], query[2]);
            }
            else if (Directory.Exists(query[1]))
            {
                if (File.Exists(query[2]))
                {
                    Log.Error("You cannot move directory into file!");
                    return;
                }
                else if (Directory.Exists(query[2]))
                    DirectoryMove(query[1], query[2]);
            }
        }
        private void DirectoryMove(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            RecursiveDirMove(new DirectoryInfo(source), new DirectoryInfo(destination));
        }
        private void RecursiveDirMove(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (var directory in Directory.GetDirectories(source.FullName))
                RecursiveDirMove(new DirectoryInfo(directory), new DirectoryInfo(Path.Combine(destination.FullName, new DirectoryInfo(directory).Name)));
            foreach (var file in Directory.GetFiles(source.FullName))
                FileMove(file, destination.FullName);
        }
        private void FileMove(string from, string to)
        {
            FileStream fromfile = File.OpenRead(from);
            FileStream tofile = File.OpenWrite(to);
            byte[] buffer = new byte[fromfile.Length];
            fromfile.Read(buffer, 0, (int)fromfile.Length);
            tofile.Write(buffer, 0, buffer.Length);
            Log.Success("Written " + buffer.Length + " bytes to file " + to);
            fromfile.Close();
            tofile.Flush();
            tofile.Close();
            buffer = new byte[0];
            File.Delete(from);
        }
    }
}
