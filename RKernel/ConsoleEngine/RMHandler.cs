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
                path = Kernel.currentPath + query[2];
            }
            catch
            {
                Log.Error("Cannot handle RM request: corrupted or incorrect request.");
                return;
            }
            if (query.Length < 3)
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
                        Directory.Delete(path, true);
                        if (!Directory.Exists(path))
                            Log.Success("Directory successfully removed!");
                        else
                            Log.Error("Cannot remove directory!");
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        if (!File.Exists(path))
                            Log.Success("File successfully removed!");
                        else
                            Log.Error("Cannot remove file!");
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
                        Directory.Delete(path);
                        if (!Directory.Exists(path))
                            Log.Success("Directory successfully removed!");
                        else
                            Log.Error("Cannot remove directory!");
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                        if (!File.Exists(path))
                            Log.Success("File successfully removed!");
                        else
                            Log.Error("Cannot remove file!");
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
