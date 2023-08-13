using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class PowerHandler
    {

        public static void HandlePowerRequest(string quer)
        {
            string[] query = quer.Split(' ');
            if (query.Length != 2)
            {
                Log.Error("Cannot handle power request");
                return;
            }
            if (query[1] == "reboot")
                Cosmos.System.Power.Reboot();
            else if (query[1] == "shutdown")
                Cosmos.System.Power.Shutdown();
        }
    }
}
