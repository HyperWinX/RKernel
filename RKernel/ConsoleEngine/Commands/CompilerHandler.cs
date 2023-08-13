using RKernel.HSMEngine;
using System.IO;

namespace RKernel.ConsoleEngine.Commands
{
    public static class CompilerHandler
    {
        public static void HandleCompilationRequest(string quer)
        {
            try
            {
                string[] query = quer.Split(' ');
                if (!File.Exists(query[1]))
                {
                    Log.Error("Cannot find file " + query[1]);
                    return;
                }
                Compiler compiler = new();
                compiler.Init();
                compiler.Compile(query[1], query[2]);
                compiler.BufferFlush();
                compiler = null;
            }
            catch
            {
                Log.Error("Compilation failure!");
            }
        }
    }
}
