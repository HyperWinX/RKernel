using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.Installer
{
    public class UserConfigurator
    {
        PGUIDriver driver;
        public UserConfigurator(PGUIDriver driver)
        {
            this.driver = driver;
        }
        public string[] Run() => driver.DrawUserCreationMenu();
    }
}
