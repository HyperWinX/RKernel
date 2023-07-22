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
            Console.Write("Enter root password: ");
            bool valid = false;
            while (!valid)
            {
                string pass = Console.ReadLine();
                
            }
        }
    }
}
