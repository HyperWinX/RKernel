using RKernel.HSMEngine;
using System;
using System.Collections.Generic;
using System.IO;
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
                Console.Write(Kernel.currentPath + "> ");
                string query = Console.ReadLine();
                if (query.StartsWith("pm "))
                {
                    try
                    {
                        string[] subq = query.Split('-');
                        for (int i = 0; i < subq.Length; i++)
                            subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                        pmhandler.HandlePMRequest(subq);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query.StartsWith("sfc "))
                {
                    try
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
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query.StartsWith("rm "))
                {
                    try
                    {
                        string[] subq = query.Split(' ');
                        for (int i = 0; i < subq.Length; i++)
                            subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                        RMHandler rmhandler = new RMHandler();
                        rmhandler.HandleRMRequest(subq);
                        rmhandler = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query.StartsWith("mkdir "))
                {
                    try
                    {
                        string[] subq = query.Split(' ');
                        for (int i = 0; i < subq.Length; i++)
                            subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                        MKDIRHandler mkdirhandler = new MKDIRHandler();
                        mkdirhandler.HandleMKDIRRequest(subq);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query == "ls")
                {
                    try
                    {
                        LSHandler lshandler = new LSHandler();
                        lshandler.HandleLSRequest();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query == "su")
                {
                    try
                    {
                        SUHandler suhandler = new SUHandler();
                        suhandler.HandleSURequest();
                        Console.WriteLine("Now you are root!");
                        suhandler = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query.StartsWith("cd "))
                {
                    try
                    {
                        string[] subq = query.Split(' ');
                        CDHandler cdhandler = new CDHandler();
                        cdhandler.HandleCDRequest(subq);
                        cdhandler = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }
                else if (query.StartsWith("mv "))
                {
                    string[] subq = query.Split(' ');
                    for (int i = 0; i < subq.Length; i++)
                        subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                    MVHandler mvhandler = new MVHandler();
                    mvhandler.HandleMVRequest(subq);
                    mvhandler = null;
                }
                else if (query.StartsWith("notepad "))
                {
                    NotepadLauncher notepadlauncher = new();
                    notepadlauncher.Run(query.Split(' '));
                }
                else if (query.StartsWith("power "))
                {
                    PowerHandler powerhandler = new();
                    powerhandler.HandlePowerRequest(query.Split(' '));
                }
                else if (query == "gui")
                {
                    RKernel.GUIEngine.MainGUI.InitializeGUI();
                }
                else if (query.StartsWith("hsm "))
                {
                    string[] subq = query.Split(' ');
                    for (int i = 0; i < subq.Length; i++)
                        subq[i] = Tools.Tools.RemoveSpaces(subq[i]);
                    if (!File.Exists(subq[1]))
                    {
                        Log.Error("Cannot find file " + subq[1]);
                        return;
                    }
                    Compiler compiler = new();
                    compiler.Init();
                    compiler.Compile(subq[1], subq[2]);
                    compiler.BufferFlush();
                }
            }
        }
    }
}
