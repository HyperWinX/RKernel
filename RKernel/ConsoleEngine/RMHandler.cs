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
            bool hasTargetArgument = false;
            bool isHelpRequest = false;
            int indexOfTargetArg = 0;
            for (int i = 1; i < query.Length; i++)
                if (query[i] == "r")
                {
                    hasRecurseArgument = true;
                    break;
                }
            for (int i = 1; i < query.Length; i++)
                if (query[i].Contains("target:"))
                {
                    hasTargetArgument = true;
                    indexOfTargetArg = i;
                    break;
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
            }
            if (!hasTargetArgument)
            {
                Log.Error("Cannot remove object: no \"target\" argument.");
                return;
            }
            if (!Directory.Exists(query[indexOfTargetArg].Substring(7)) && !File.Exists(query[indexOfTargetArg].Substring(7)))
            {
                Log.Error("Cannot remove object: cannot find object " + query[indexOfTargetArg].Substring(7));
                return;
            }
            try
            {
                if (hasRecurseArgument)
                {
                    if (Directory.Exists(query[indexOfTargetArg].Substring(7)))
                    {
                        Directory.Delete(query[indexOfTargetArg].Substring(7), true);
                        if (!Directory.Exists(query[indexOfTargetArg].Substring(7)))
                            Log.Success("Directory successfully removed!");
                        else
                            Log.Error("Cannot remove directory!");
                    }
                    else if (File.Exists(query[indexOfTargetArg].Substring(7)))
                    {
                        File.Delete(query[indexOfTargetArg].Substring(7));
                        if (!File.Exists(query[indexOfTargetArg].Substring(7)))
                            Log.Success("File successfully removed!");
                        else
                            Log.Error("Cannot remove file!");
                    }
                    else
                    {
                        Log.Error("Cannot remove object: cannot find object " + query[indexOfTargetArg].Substring(7));
                        return;
                    }
                }
                else
                {
                    if (Directory.Exists(query[indexOfTargetArg].Substring(7)))
                    {
                        Directory.Delete(query[indexOfTargetArg].Substring(7));
                        if (!Directory.Exists(query[indexOfTargetArg].Substring(7)))
                            Log.Success("Directory successfully removed!");
                        else
                            Log.Error("Cannot remove directory!");
                    }
                    else if (File.Exists(query[indexOfTargetArg].Substring(7)))
                    {
                        File.Delete(query[indexOfTargetArg].Substring(7));
                        if (!File.Exists(query[indexOfTargetArg].Substring(7)))
                            Log.Success("File successfully removed!");
                        else
                            Log.Error("Cannot remove file!");
                    }
                    else
                    {
                        Log.Error("Cannot remove object: cannot find object " + query[indexOfTargetArg].Substring(7));
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
