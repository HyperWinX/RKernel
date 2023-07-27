using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class RMHandler
    {
        public RMHandler() { }
        public void HandleRMRequest(string[] query)
        {
            if (query.Length < 2 || query.Length > 3)
            {
                Log.Error("Cannot handle RM request: corrupted or incorrect request.");
                return;
            }
            bool hasRecurseArgument = false;
            bool isHelpRequest = false;
            string path;
            for (int i = 1; i < query.Length; i++)
                if (query[i] == "-r")
                {
                    hasRecurseArgument = true;
                    break;
                }
            try
            {
                path = Kernel.currentPath + query.Last();
            }
            catch
            {
                Log.Error("Cannot handle RM request: corrupted or incorrect request.");
                return;
            }
            for (int i = 1; i < query.Length; i++)
            {
                if (query[i].Contains("help"))
                {
                    isHelpRequest = true;
                    break;
                }
            }
            if (isHelpRequest)
            {
                Console.Write("\nUsage:\n");
                Console.WriteLine("rm -r -target:0:\folder - Remove target recursively");
                Console.WriteLine("rm -target:0:\file.txt - Remove target\n");
                return;
            }
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Log.Error("Cannot remove object: cannot find object " + path);
                return;
            }
            try
            {
                if (hasRecurseArgument)
                {
                    if (Directory.Exists(path))
                    {
                        if (Kernel.ProtectedPaths.Contains(path.GetHashCode()) && !Kernel.IsRoot)
                        {
                            Log.Error("Not enough permissions to remove directory! Exiting...");
                            return;
                        }
                        try { Directory.Delete(path, true); } catch (Exception ex) { Log.Error(ex.Message); }
                    }
                    else if (File.Exists(path))
                    {
                        Log.Error("Cannot remove file with -r argument!");
                        return;
                    }
                    else
                    {
                        Log.Error("Cannot remove object: cannot find object " + path);
                        return;
                    }
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        if (Kernel.ProtectedPaths.Contains(path.GetHashCode()) && !Kernel.IsRoot)
                        {
                            Log.Error("Not enough permissions to remove directory! Exiting...");
                            return;
                        }
                        try { Directory.Delete(path); } catch (Exception ex) { Log.Error(ex.Message); }
                    }
                    else if (File.Exists(path))
                    {
                        if (Kernel.ProtectedPaths.Contains(path.GetHashCode()) && !Kernel.IsRoot)
                        {
                            Log.Error("Not enough permissions to remove file! Exiting...");
                            return;
                        }
                        try { File.Delete(path); } catch (Exception ex) { Log.Error(ex.Message); return; }
                    }
                    else
                    {
                        Log.Error("Cannot remove object: cannot find object " + path);
                        return;
                    }
                }
            } catch (Exception ex)
            {
                Log.Error("Unknown exception: \n");
                Log.Error(ex.Message);
            }
        }
    }
}