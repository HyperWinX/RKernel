using Cosmos.HAL;
using Cosmos.System.Graphics;
using RKernel.ConsoleEngine.Commands;
using RKernel.HSMEngine;
using System;
using System.Collections;
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
        private Dictionary<string, Action<string>> _commands;
        public Engine() { }

        public void RegisterCommands()
        {
            _commands = new Dictionary<string, Action<string>>
            {
                {"cd", CDHandler.HandleCDRequest },
                {"ls", LSHandler.HandleLSRequest },
                {"mkdir", MKDIRHandler.HandleMKDIRRequest },
                {"mv", MVHandler.HandleMVRequest },
                {"notepad", NotepadLauncher.Run },
                {"pm", PMHandler.HandlePMRequest },
                {"power", PowerHandler.HandlePowerRequest },
                {"rm", RMHandler.HandleRMRequest },
                {"su", SUHandler.HandleSURequest },
                {"hsm", CompilerHandler.HandleCompilationRequest },
                {"sfc", SFCHandler.HandleSFCRequest },
                {"ctl", CTLHandler.HandleCTLRequest }
            };
        }
        public void RunEngine()
        {
            while (true)
            {
                Console.Write(Kernel.currentPath + "> ");
                HandleCommand(Console.ReadLine());
            }
        }
        public void HandleCommand(string command)
        {
            try
            {
                if (command.StartsWith("./"))
                {
                    if (File.Exists(Path.Combine(Kernel.currentPath, command.Substring(2))))
                    {
                        Runner runner = new(Path.Combine(Kernel.currentPath, command.Substring(2)), Kernel.IsDebugMode, Kernel.RunnerMemorySize);
                        runner.Load();
                        runner.Run();
                    }
                    else
                        Log.Error("Cannot start file: file not found!");
                    return;
                }
                _commands[command.Split(' ')[0]](command);
            }
            catch
            {
                Log.Error("Command handling failure!");
            }
        }
    }
}
