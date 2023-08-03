using System;
using System.IO;
using RKernel.Applications;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.ConsoleEngine
{
    public class NotepadLauncher
    {
        public NotepadLauncher() { }
        public void Run(string[] query)
        {
            if (query.Length != 2)
            {
                Log.Error("Cannot launch notepad: argument parsing failed.");
                return;
            }
            if (File.Exists(query[1]) || File.Exists(Kernel.currentPath + query[1]) || File.Exists(Kernel.currentPath + "\\" + query[1]))
            {
                Notepad.Run(new string[2] { "notepad", query[1] });
            }
            try
            {
                File.Create(query[1]).Close();
                Notepad.Run(new string[2] { "notepad", query[1] });
            } catch { }
        }
    }
}
