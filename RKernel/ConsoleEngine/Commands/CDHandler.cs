using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class CDHandler
    {
        public static void HandleCDRequest(string quer)
        {
            string[] query = quer.Split(' '); 
            if (query[0] != "cd")
            {
                Log.Error("Cannot handle CD request: corrupted, or incorrect request.");
                return;
            }
            if (query.Length != 2)
            {
                Log.Error("Cannot handle CD request: corrupted, or incorrect request.");
                return;
            }
            if (Directory.GetDirectories(Kernel.currentPath).Contains(query[1]))
                Kernel.currentPath += query[1] + "\\";
            if (query[1] == "..")
            {
                if (Kernel.currentPath == "0:\\")
                    return;
                else
                    try
                    {
                        Kernel.currentPath = Directory.GetParent(Kernel.currentPath).FullName;
                    }
                    catch (Exception ex) { Log.Error(ex.Message); return; }
            }
        }
    }
}
