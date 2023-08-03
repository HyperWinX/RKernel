using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HASMEditorAndCompiler
{
    internal static class Compiler
    {
        private static List<byte> externSection;
        private static List<byte> dataSection;
        private static List<byte> mainSection;
        private static Dictionary<string, byte> registers;
        private static List<string> externs;
        public static void Init()
        {
            externSection = new List<byte>();
            dataSection = new List<byte>();
            mainSection = new List<byte>();
            registers = new Dictionary<string, byte>
            {
                {"eax", 0xC0},
                {"ebx", 0xC1},
                {"ecx", 0xC2},
                {"edx", 0xC3},
                {"ax", 0xC4},
                {"bx", 0xC5},
                {"cx", 0xC6},
                {"dx", 0xC7},
                {"si", 0xC8}
            };
            externs = new List<string>
            {
                "print",
                "read",
                "beep"
            };
        }
        public static void Compile(string targetfile, string outputto)
        {
            List<string> lines = File.ReadAllLines(targetfile).ToList();
            ClearEmptyLines(ref lines);
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.StartsWith("extern "))
                {
                    string[] sline = line.Split(' ');
                    if (sline.Length != 2)
                    {
                        //err
                        Console.WriteLine("Incorrect arguments");
                        return;
                    }
                    if (!externs.Contains(sline[1]))
                    {
                        //err
                        Console.WriteLine("Cannot find extern");
                        return;
                    }
                    externSection.Add(0xFF);
                    byte ext = DefineExtern(sline[1]);
                    if (ext == 0x00)
                    {
                        //err
                        Console.WriteLine("Unknown extern");
                        return;
                    }
                    externSection.Add(ext);
                }
                else if (line.StartsWith("dd "))
                {
                    string varname = line.Substring(3);
                    List<string> t = new List<string> { varname };
                    ClearEmptyLines(ref t);
                    if (t.Count == 0)
                    {
                        //Log error
                        Console.WriteLine("Null variable name, compilation exited");
                        Dispose();
                        return;
                    }
                    dataSection.Add(0xE0);
                    dataSection.Add((byte)t[0].Length);
                    foreach (byte b in Encoding.ASCII.GetBytes(t[0]))
                        dataSection.Add(b);
                }
                else if (line.StartsWith("dw "))
                {
                    string varname = line.Substring(3);
                    List<string> t = new List<string> { varname };
                    ClearEmptyLines(ref t);
                    if (t.Count == 0)
                    {
                        //Log error
                        Console.WriteLine("Null variable name, compilation exited");
                        Dispose();
                        return;
                    }
                    dataSection.Add(0xD0);
                    dataSection.Add((byte)t[0].Length);
                    foreach (byte b in Encoding.ASCII.GetBytes(t[0]))
                        dataSection.Add(b);
                }
                else if (line.StartsWith("mov "))
                {
                    string[] sline = line.Split(' ');
                    Console.WriteLine(sline[0]);
                    Console.WriteLine(sline[1]);
                    Console.WriteLine(sline[2]);
                    if (sline.Length != 3 && !sline[2].StartsWith("\"") && !sline.Last().EndsWith("\""))
                    {
                        //error
                        Console.WriteLine("Too much or too many arguments for mov command");
                        return;
                    }
                    mainSection.Add(0x00);
                    if (registers.Keys.ToArray().Contains(sline[1]))
                        mainSection.Add(registers[sline[1]]);
                    else if (sline[1].StartsWith("[") && sline[1].EndsWith("]"))
                        WriteVariableName(ref mainSection, sline[1]);
                    else if (sline[1].StartsWith("0x"))
                    {
                        //error
                        Console.WriteLine("Cannot put something into value");
                        return;
                    }
                    else if (sline[1].StartsWith("addr:0x"))
                    {
                        mainSection.Add(0xB1);
                        mainSection.Add(BitConverter.GetBytes(Convert.ToInt32(sline[1].Substring(7)))[0]);
                        mainSection.Add(BitConverter.GetBytes(Convert.ToInt32($"{sline[1][9]}{sline[1][10]}"))[0]);
                        Console.WriteLine(BitConverter.GetBytes(Convert.ToInt32(sline[1].Substring(7)))[0]);
                    }
                    if (registers.Keys.ToArray().Contains(sline[2]))
                        mainSection.Add(registers[sline[2]]);
                    else if (sline[2].StartsWith("[") && sline[2].EndsWith("]"))
                        WriteVariableName(ref mainSection, sline[2]);
                    else if (sline[2].StartsWith("\"") && sline.Last().EndsWith("\""))
                    {
                        if (sline[1] != "si")
                        {
                            //err
                            Console.WriteLine("Cannot put string into not si register");
                            return;
                        }
                        string ssline = "";
                        string[] g = sline;
                        if (sline.Length > 3)
                        {
                            for (int j = 2; j < sline.Length; j++)
                            {
                                ssline += sline[j];
                                ssline += " ";
                            }
                        }
                        else
                            ssline = sline[2].Substring(1, sline[2].Length - 2);
                        ssline = ssline.Substring(1, ssline.Length - 3);
                        //ssline = ssline.Remove(ssline.Length - 1, 1);
                        //ssline = ssline.Substring(0, ssline.Length - 1);
                        mainSection.Add(0xF0);
                        mainSection.Add((byte)ssline.Length);
                        foreach (byte b in Encoding.ASCII.GetBytes(ssline))
                            mainSection.Add(b);
                    }
                    else if (sline[2].StartsWith("0x"))
                    {
                        int bytecount = sline[2].Substring(2).Length / 2;
                        mainSection.Add(0xB0);
                        mainSection.Add((byte)bytecount);
                        for (int j = 2; j < bytecount * 2 + 1; j += 2)
                            mainSection.Add(Convert.ToByte($"{sline[2][j]}{sline[2][j + 1]}", 16));
                    }
                    else if (sline[2].StartsWith("addr:0x"))
                    {
                        mainSection.Add(0xB1);
                        mainSection.Add(BitConverter.GetBytes(Convert.ToInt32(sline[1].Substring(7)))[0]);
                        mainSection.Add(BitConverter.GetBytes(Convert.ToInt32($"{sline[1][9]}{sline[1][10]}"))[0]);
                    }
                    else
                    {
                        byte[] bytes = ConvertStringToByte(sline[2]);
                        mainSection.Add(0xB0);
                        mainSection.Add((byte)bytes.Length);
                        foreach (byte b in bytes)
                            mainSection.Add(b);
                    }
                }
                else if (line == "print")
                {
                    mainSection.Add(0xA0);
                }
                else if (line == "beep")
                {
                    mainSection.Add(0xA2);
                }
            }
            List<byte> finalfile = new List<byte>();
            for (int j = 0; j < externSection.Count; j++)
                finalfile.Add(externSection[j]);
            for (int j = 0; j < dataSection.Count; j++)
                finalfile.Add(dataSection[j]);
            for (int j = 0; j < mainSection.Count; j++)
                finalfile.Add(mainSection[j]);
            finalfile.Add(0xB2);
            finalfile.Add(0xBB);
            File.WriteAllBytes(outputto, finalfile.ToArray());
            finalfile.Clear();
        }
        private static void ClearEmptyLines(ref List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) || string.IsNullOrWhiteSpace(lines[i]))
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }
        }
        private static void WriteVariableName(ref List<byte> mainSection, string varname)
        {
            List<string> t = new List<string> { varname };
            ClearEmptyLines(ref t);
            if (t.Count == 0)
            {
                //Log error
                Console.WriteLine("Null variable name, compilation exited");
                Dispose();
                return;
            }
            mainSection.Add((byte)t[0].Length);
            foreach (byte b in Encoding.ASCII.GetBytes(t[0]))
                mainSection.Add(b);
        }
        private static byte[] ConvertStringToByte(string bytestr) => Encoding.ASCII.GetBytes(bytestr);
        private static byte DefineExtern(string externstr)
        {
            switch (externstr)
            {
                case "print":
                    return 0xA0;
                case "read":
                    return 0xA1;
                case "beep":
                    return 0xA2;
                default:
                    return 0x00;
            }
        }
        private static void Dispose()
        {
            externSection.Clear(); externSection = null;
            dataSection.Clear(); dataSection = null;
            mainSection.Clear(); dataSection = null;
        }
    }
}
