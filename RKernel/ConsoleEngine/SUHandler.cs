using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class SUHandler
    {
        public SUHandler() { }
        public void HandleSURequest()
        {
            bool valid = false;
            while (!valid)
            {
                Console.Write("Enter root password: ");
                string pass = Console.ReadLine();
                if (pass == Kernel.RootPasswd)
                    valid = true;
            }
            Kernel.currentMode = "[root]";
            Kernel.IsRoot = true;
        }
    }
}
