using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class PowerHandler
    {
        public PowerHandler() { }

        public void HandlePowerRequest(string[] query)
        {
            if (query.Length != 2)
            {
                Log.Error("Cannot handle power request");
                return;
            }
            if (query[1] == "reboot")
            {
                Cosmos.System.Power.Reboot();
            }
            else if (query[1] == "shutdown")
            {
                Cosmos.System.Power.Shutdown();
            }
        }
    }
}
