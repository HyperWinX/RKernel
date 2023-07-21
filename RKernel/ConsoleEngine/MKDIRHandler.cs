using System;
using System.IO;

namespace RKernel.ConsoleEngine
{
    public class MKDIRHandler
    {
        public MKDIRHandler() { }
        public void HandleMKDIRRequest(string[] query)
        {
            if (query[0] != "mkdir")
            {
                Log.Error("Cannot handle MKDIR request: corrupted, or incorrect request.");
                return;
            }
            if (query[1].Substring(1).StartsWith(":\\"))
            {
                if (Directory.Exists(query[1]))
                {
                    Log.Warning("Directory already exists!");
                    return;
                }
                else if (File.Exists(query[1]))
                {
                    Log.Warning("Target existt, but its not a directory.");
                    return;
                }
                try
                {
                    Directory.CreateDirectory(query[1]);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot create directory: " + ex.Message);
                }
            }
            else
            {
                if (Directory.Exists(Kernel.currentPath + query[1]))
                {
                    Log.Warning("Directory already exists!");
                    return;
                }
                else if (File.Exists(Kernel.currentPath + query[1]))
                {
                    Log.Warning("Target existt, but its not a directory.");
                    return;
                }
                try
                {
                    Directory.CreateDirectory(Kernel.currentPath + query[1]);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot create directory: " + ex.Message);
                }
            }
        }
    }
}
