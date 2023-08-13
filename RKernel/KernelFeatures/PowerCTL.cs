using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.KernelFeatures
{
    public class PowerCTL
    {
        public PowerCTL() { }
        public void Shutdown()
        {
            Kernel.SystemConfig.Save();
            Kernel.UserConfig.Save();
            Cosmos.System.Power.Shutdown();
        }
        public void Reboot()
        {
            Kernel.SystemConfig.Save();
            Kernel.UserConfig.Save();
            Cosmos.System.Power.Reboot();
        }
    }
}
