using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.HSMEngine
{
    internal static class Runner
    {
        private static Dictionary<string, List<byte>> Registers32bit = new Dictionary<string, List<byte>>
        {
            {"eax", new List<byte>(4)},
            {"ebx", new List<byte>(4)},
            {"ecx", new List<byte>(4)},
            {"edx", new List<byte>(4)}
        };
        private static Dictionary<string, List<byte>> Registers16bit = new Dictionary<string, List<byte>>
        {
            {"ax", new List<byte>(2)},
            {"bx", new List<byte>(2)},
            {"cx", new List<byte>(2)},
            {"dx", new List<byte>(2)}
        };
        private static Dictionary<string, List<byte>> OtherRegisters = new Dictionary<string, List<byte>>
        {
            {"si", new List<byte>(512)}
        };
        private static List<byte> Memory = new List<byte>(65536);
        private static List<byte> SupportedExternCodes = new List<byte>
        {
            0xA0,
            0xA1,
            0xA2
        };
        private static List<string> Externs = new List<string>();
        public static void Run(string filepath)
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            for (int i = 0; i < bytes.Length; i++)
                Memory[i] = bytes[i];
            short pointer = 0x0000;
            bytes = null;
            while (Memory[pointer] != 0xB2 && Memory[pointer + 1] != 0xBB)
            {

            }
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.WriteLine("Current byte: " + (i + 1) + " , " + bytes[i]);
                if (bytes[i] == 0xFF)
                {
                    if (!SupportedExternCodes.Contains(bytes[i + 1]))
                    {
                        //err
                        Console.WriteLine("Unsupported extern code");
                        return;
                    }
                    Externs.Add(DefineExternType(bytes[i + 1]));
                    i++;
                }
                else if (bytes[i] == 0x00)
                {
                    if (bytes[i + 1] == 0xD0) { }
                    else if (bytes[i + 1] == 0xE0) { }
                    else
                    {
                        string strtoset = "";
                        string register = DefineRegister(bytes[i + 1]);
                        Console.WriteLine("Register: " + register);
                        Console.WriteLine(0xF0);
                        Console.WriteLine(bytes[i + 2]);
                        if (register == null && bytes[i + 2] == 0xF0)
                        {
                            int length = bytes[i + 3];
                            List<byte> str = new List<byte>();
                            for (int j = 0; i < length; i++)
                                str.Add(bytes[i + 3 + j]);
                            string decodedStr = Encoding.ASCII.GetString(str.ToArray());
                            strtoset = decodedStr.Substring(1, decodedStr.Length - 2);
                        }
                        string register2 = DefineRegister(bytes[i + 2]);
                        if (register == null && bytes[i + 2] != 0xF0)
                        {
                            //error
                            Console.WriteLine("Unknown register, exiting...");
                            return;
                        }
                        if (Registers32bit.Keys.ToArray().Contains(register) && Registers32bit.Keys.ToArray().Contains(register2))
                        {
                            Registers32bit[register] = Registers32bit[register2];
                            Console.WriteLine("Moving " + register2 + " to register " + register);
                            i += 2;
                        }
                        else if (Registers16bit.Keys.ToArray().Contains(register) && Registers16bit.Keys.ToArray().Contains(register2))
                        {
                            Console.WriteLine("Moving " + register2 + " to register " + register);
                            i += 2;
                        }
                        else if (Registers32bit.Keys.ToArray().Contains(register) && bytes[i + 2] == 0xB0)
                        {
                            Console.WriteLine("Detected data start");
                            List<byte> value = new List<byte>();
                            int count = bytes[i + 3];
                            for (int j = 0; j < count; j++)
                                value.Add(bytes[i + 4 + j]);
                            SetRegisterValue(register, value);
                            i += 3 + count;
                        }
                        else if (Registers32bit.Keys.ToArray().Contains(register) && strtoset != "")
                        {
                            Console.WriteLine("null");
                        }
                        else if (OtherRegisters.Keys.ToArray().Contains(register) && bytes[i + 2] == 0xF0)
                        {
                            int count = bytes[i + 3];
                            List<byte> str = new List<byte>();
                            for (int j = 4; j < count; j++)
                                str.Add(bytes[i + j]);
                            str.RemoveAt(str.Count - 1);
                            SetRegisterValue(register, str);
                            i += 3 + count;
                        }
                    }
                }
                else if (bytes[i] == 0xA0)
                {
                    if (!Externs.Contains("print"))
                    {
                        //err
                        Console.WriteLine("Unknown extern");
                        return;
                    }
                    if (Registers32bit["eax"].Count != 4)
                    {
                        foreach (byte b in Registers32bit["eax"])
                            Console.WriteLine(b);
                        //err
                        Console.WriteLine("Null eax register");
                        return;
                    }
                    if (Registers32bit["ebx"].Count != 4)
                    {
                        //err
                        Console.WriteLine("Null ebx register");
                        return;
                    }
                    ConsoleColor ForegroundColor = DefineForegroundColor(Registers32bit["eax"]);
                    if (ForegroundColor == ConsoleColor.Black)
                    {
                        //err
                        Console.WriteLine("Unknown foreground color");
                        return;
                    }
                    ConsoleColor BackgroundColor = DefineBackgroundColor(Registers32bit["ebx"]);
                    if (BackgroundColor == ConsoleColor.White)
                    {
                        //err
                        Console.WriteLine("Unknown background color");
                        return;
                    }
                    //TODO: write si value to console with colors
                    string str = Encoding.ASCII.GetString(OtherRegisters["si"].ToArray());
                    ConsoleColor[] old = new ConsoleColor[2] { Console.ForegroundColor, Console.BackgroundColor };
                    Console.ForegroundColor = ForegroundColor;
                    Console.BackgroundColor = BackgroundColor;
                    Console.WriteLine(str);
                    Console.ForegroundColor = old[0];
                    Console.BackgroundColor = old[1];
                }
                else if (bytes[i] == 0xA2)
                {
                    if (!Externs.Contains("beep"))
                    {
                        //err
                        Console.WriteLine("Unknown extern");
                        return;
                    }
                    Console.Beep();
                    i++;
                }
                else if (bytes[i] == 0xB2 && bytes[i + 1] == 0xBB && i + 2 == bytes.Length)
                    return;
            }
        }
        private static string DefineRegister(byte b)
        {
            switch (b)
            {
                case 0xC0:
                    return "eax";
                case 0xC1:
                    return "ebx";
                case 0xC2:
                    return "ecx";
                case 0xC3:
                    return "edx";
                case 0xC4:
                    return "ax";
                case 0xC5:
                    return "bx";
                case 0xC6:
                    return "cx";
                case 0xC7:
                    return "dx";
                case 0xC8:
                    return "si";
                default:
                    return null;
            }
        }
        private static byte[] ConvertStringToByte(string bytestr)
        {
            int length = (bytestr.Length - 2) / 2;
            byte[] byteArray = new byte[length];
            string hexSubstring = bytestr.Substring(2);

            for (int i = 0; i < length; i++)
            {
                byteArray[i] = Convert.ToByte(hexSubstring.Substring(i * 2, 2), 16);
            }

            return byteArray;
        }
        private static string DefineExternType(byte externbyte)
        {
            switch (externbyte)
            {
                case 0xA0:
                    return "print";
                case 0xA1:
                    return "read";
                case 0xA2:
                    return "beep";
                default:
                    return null;
            }
        }
        private static ConsoleColor DefineForegroundColor(List<byte> bytes)
        {
            Console.WriteLine(bytes[0]);
            Console.WriteLine(bytes[1]);
            Console.WriteLine(bytes[2]);
            Console.WriteLine(bytes[3]);
            if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x00)
                return ConsoleColor.White;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x01)
                return ConsoleColor.Green;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x02)
                return ConsoleColor.Red;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x03)
                return ConsoleColor.Blue;
            else
                return ConsoleColor.Black;
        }
        private static ConsoleColor DefineBackgroundColor(List<byte> bytes)
        {
            if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x00)
                return ConsoleColor.Black;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x01)
                return ConsoleColor.Green;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x02)
                return ConsoleColor.Red;
            else if (bytes[0] == 0x00 &&
                bytes[1] == 0x00 &&
                bytes[2] == 0x00 &&
                bytes[3] == 0x03)
                return ConsoleColor.Blue;
            else
                return ConsoleColor.White;
        }
        private static void ResetRegister(string register)
        {

        }
        private static void SetRegisterValue(string register, List<byte> value)
        {
            if (register == "eax" || register == "ebx" || register == "ecx" || register == "edx")
            {
                Console.WriteLine("Setting register " + register + "");
                Console.WriteLine($"{value[0]} {value[1]} {value[2]} {value[3]}");
                if (value.Count == 4)
                    Registers32bit[register] = value;
                else if (value.Count < 4 || value.Count > 0)
                {
                    Console.WriteLine("Setting " + register);
                    foreach (byte b in value)
                        Console.WriteLine(b);
                    for (int j = 0; j < value.Count; j++)
                        Registers32bit[register][j] = value[j];
                }
                else
                {
                    //err
                    Console.WriteLine("Cannot set register");
                }
            }
            else if (register == "ax" || register == "bx" || register == "cx" || register == "dx")
            {
                if (value.Count == 2)
                    Registers16bit[register] = value;
                else if (value.Count == 1)
                    Registers16bit[register][0] = value[0];
                else
                {
                    //err
                    Console.WriteLine("Cannot set register");
                }
            }
            else if (register == "si")
            {
                OtherRegisters[register] = value;
            }
            else
            {
                //err
                Console.WriteLine("Cannot find register");
            }
        }
    }
}
