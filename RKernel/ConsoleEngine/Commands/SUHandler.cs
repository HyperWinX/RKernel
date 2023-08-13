using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine.Commands
{
    public static class SUHandler
    {
        public static void HandleSURequest(string quer)
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
