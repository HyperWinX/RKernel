using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class MKDIRHandler
    {
        public MKDIRHandler() { }
        public void HandleMKDIRRequest(string[] query)
        {
            if (query[0] != "mkdir")
            {
                Log.Error("Cannot handle MKDIR request: corrupted, or incorrect request.");
                return;
            }
            switch (query[1])
            {

            }
        }
    }
}
