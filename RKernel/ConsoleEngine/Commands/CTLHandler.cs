using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class CTLHandler
    {
        public static void HandleCTLRequest(string query)
        {
            string[] splitted = query.Split(' ');
            if (splitted.Length < 2 || splitted.Length > 4)
            {
                Log.Error("CTL run failure");
                return;
            }
            if (splitted.Length == 3)
            {
                switch (splitted[1])
                {
                    case "power":
                        switch (splitted[2])
                        {
                            case "shutdown":
                                Kernel.PWRController.Shutdown();
                                break;
                            case "reboot":
                                Kernel.PWRController.Reboot();
                                break;
                        }
                        break;
                    case "get":
                        try
                        {
                            Console.WriteLine(Kernel.SystemConfig[splitted[2]]);
                        }
                        catch
                        {
                            Log.Error("Cannot get value of variable " + splitted[2]);
                        }
                        break;
                }
            }
            else if (splitted.Length == 4)
            {
                switch (splitted[1])
                {
                    case "set":
                        if (Kernel.SystemConfig.GetConfigEntries().Contains(splitted[2]))
                            Kernel.SystemConfig.AddPair(splitted[2], splitted[3]);
                        else
                            Kernel.SystemConfig[splitted[2]] = splitted[3];
                        break;
                }
            }
        }
    }
}
