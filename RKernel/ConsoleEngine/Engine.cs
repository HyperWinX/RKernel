using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class Engine
    {
        private PMHandler pmhandler;
        public Engine() { pmhandler = new PMHandler(); }

        public void RunEngine()
        {
            while (true)
            {
                Console.Write(">>> ");
                string query = Console.ReadLine();
                if (query.StartsWith("pm "))
                {
                    string[] subq = query.Split('-');
                    for (int i = 0; i < subq.Length; i++)
                        subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                    pmhandler.HandlePMRequest(subq);
                }
                else if (query.StartsWith("sfc "))
                {
                    string[] subq = query.Split('-');
                    for (int i = 0; i < subq.Length; i++)
                        subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                    SFCHandler sfchandler = new SFCHandler();
                    List<int> errors = sfchandler.HandleSFCRequest(subq);
                    if (errors == null)
                        continue;
                    if (errors.Count == 0)
                    {
                        Log.Success("No errors detected!");
                    }
                    else
                    {
                        Log.Warning("Detected errors:");
                        for (int i = 0; i < errors.Count; i++)
                        {
                            Log.Warning(ErrorIDs.Ids[errors[i]]);
                        }
                    }
                }
                else if (query.StartsWith("rm "))
                {
                    string[] subq = query.Split('-');
                    for (int i = 0; i < subq.Length; i++)
                        subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                    RMHandler rmhandler = new RMHandler();
                    rmhandler.HandleRMRequest(subq);
                    rmhandler = null;
                }
                else if (query.StartsWith("installer"))
                {
                    Installer.Installer installer = new Installer.Installer();
                    installer.Run();
                }
            }
        }
    }
}
