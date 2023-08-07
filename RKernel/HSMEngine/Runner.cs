using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.HSMEngine
{
    internal static class Runner
    {
        private static Dictionary<int, List<byte>> Registers32bit = new Dictionary<int, List<byte>>
        {
            {"eax".GetHashCode(), new List<byte>(4)},
            {"ebx".GetHashCode(), new List<byte>(4)},
            {"ecx".GetHashCode(), new List<byte>(4)},
            {"edx".GetHashCode(), new List<byte>(4)}
        };
        private static Dictionary<int, List<byte>> Registers16bit = new Dictionary<int, List<byte>>
        {
            {"ax".GetHashCode(), new List<byte>(2)},
            {"bx".GetHashCode(), new List<byte>(2)},
            {"cx".GetHashCode(), new List<byte>(2)},
            {"dx".GetHashCode(), new List<byte>(2)}
        };
        private static Dictionary<int, List<byte>> OtherRegisters = new Dictionary<int, List<byte>>
        {
            {"si".GetHashCode(), new List<byte>(512)}
        };
        private static VirtualMemory Memory = new VirtualMemory(65536);
        private static List<byte> SupportedExternCodes = new List<byte>
        {
            0xA0,
            0xA1,
            0xA2
        };
        private static List<int> Externs = new List<int>();
        private static bool Registers4BContains(int hashcode) => Registers32bit.Keys.ToArray().Contains(hashcode);
        private static bool Registers2BContains(int hashcode) => Registers16bit.Keys.ToArray().Contains(hashcode);
        public static void Run(string filepath)
        {
            Memory.Load(File.ReadAllBytes(filepath), 0);
            short pointer = 0x0000;
            Registers4BContains(0);
            Log.Warning("Runner initialized, starting execution...");
            while (Memory[pointer] != 0xB2 && Memory[pointer + 1] != 0xBB)
            {
                Console.WriteLine("Current byte: " + (pointer + 1) + " , " + Memory[pointer] + ", converted: " + $"0x{Memory[pointer].ToString("X2")}");
                if (Memory[pointer] == 0xFF)
                {
                    if (!SupportedExternCodes.Contains(Memory[pointer + 1]))
                    {
                        Log.Error("Unsupported extern code detected! Shutting down virtual environment...");
                        //TODO: disposing and cleaning up VE
                        return;
                    }
                    Externs.Add(DefineExternType(Memory[pointer + 1]).GetHashCode());
                    pointer += 2;
                }
                else if (Memory[pointer] == 0x00)
                {
                    if (Memory[pointer + 1] == 0xD0) { }
                    else if (Memory[pointer + 1] == 0xE0) { }
                    else
                    {
                        string strtoset = "";
                        string r1 = DefineRegister(Memory[pointer + 1]);
                        object[] register = new object[3] {string.IsNullOrWhiteSpace(r1) ? false : true, r1, string.IsNullOrWhiteSpace(r1) ? 0 : r1.GetHashCode() };
                        Console.WriteLine("Register: " + r1);
                        Console.WriteLine(0xF0 + ", converted: " + $"0x{0xF0.ToString("X2")}");
                        Console.WriteLine(Memory[pointer + 2] + ", converted: " + $"0x{Memory[pointer + 2].ToString("X2")}");
                        if (register == null && Memory[pointer + 2] == 0xF0)
                        {
                            int length = Memory[pointer + 3];
                            List<byte> str = new List<byte>();
                            for (int i = 0; i < length; i++)
                                str.Add(Memory[pointer + 3 + i]);
                            strtoset = Encoding.ASCII.GetString(str.ToArray()).Substring(1, str.ToArray().Length - 2);
                        }
                        string r2 = DefineRegister(Memory[pointer + 2]);
                        object[] register2 = new object[3] { string.IsNullOrWhiteSpace(r2) ? false : true, r2, string.IsNullOrWhiteSpace(r2) ? 0 : r2.GetHashCode() };
                        Console.WriteLine("Defined second register: " + r2 + ", register code: " + Memory[pointer + 2].ToString("X2"));
                        if (register == null && Memory[pointer + 2] != 0xF0)
                        {
                            Log.Error("Unknown register code, shutting down virtual environment...");
                            //TODO: disposing and cleaning up
                            return;
                        }
                        Log.Warning("Register check passed!");
                        if ((bool)register[0] && (bool)register2[0])
                        {
                            if (Registers4BContains((int)register[2]) && Registers4BContains((int)register2[2]))
                            {
                                Registers32bit[(int)register[2]] = Registers32bit[(int)register2[2]];
                                Console.WriteLine("Moving " + register2 + " to register " + register);
                                pointer += 3;
                            }
                            else if (Registers2BContains((int)register[2]) && Registers2BContains((int)register2[2]))
                            {
                                Registers16bit[(int)register[2]] = Registers16bit[(int)register2[2]];
                                Console.WriteLine("Moving " + register2 + " to register " + register);
                                pointer += 3;
                            }
                            else if (Registers2BContains((int)register[2]) && Registers4BContains((int)register2[2]))
                            {
                                SetRegisterValue((int)register[2], new List<byte>(2) { Registers32bit[(int)register[2]][2], Registers32bit[(int)register[2]][3] });
                                pointer += 3;
                            }
                            else if (Registers4BContains((int)register[2]) && Registers2BContains((int)register2[2]))
                            {
                                SetRegisterValue((int)register[2], Registers32bit[(int)register2[2]]);
                                pointer += 3;
                            }
                            else
                                throw new Exception("You are lucky, you got inaccessible exception!");
                        }
                        else if (!(bool)register[0] && (bool)register2[0])
                        {
                            if (Memory[pointer + 1] == 0xB1)
                            {
                                int memoryAddress = BitConverter.ToInt16(new byte[2] { Memory[pointer + 2], Memory[pointer + 3] });
                                
                            }
                        }
                        else if (Registers4BContains((int)register[2]) && Memory[pointer + 2] == 0xB0)
                        {
                            Console.WriteLine("Detected data start");
                            List<byte> value = new List<byte>();
                            int count = Memory[pointer + 3];
                            for (int i = 0; i < count; i++)
                                value.Add(Memory[pointer + 4 + i]);
                            SetRegisterValue(register, value);
                            pointer += (Int16)(4 + count);
                        }
                        else if (Registers32bit.Keys.ToArray().Contains(register.GetHashCode()) && strtoset != "")
                        {
                            Console.WriteLine("null");
                        }
                        else if (OtherRegisters.Keys.ToArray().Contains(register.GetHashCode()) && Memory[pointer + 2] == 0xF0)
                        {
                            int count = Memory[pointer + 3];
                            List<byte> str = new List<byte>();
                            for (int i = 4; i < count; i++)
                                str.Add(Memory[pointer + i]);
                            str.RemoveAt(str.Count - 1);
                            SetRegisterValue(register, str);
                            pointer += (Int16)(4 + count);
                        }
                        Log.Warning("Bro, i cant handle this fucking (byte)shit");
                    }
                }
                else if (Memory[pointer] == 0xA0)
                {
                    if (!Externs.Contains("print".GetHashCode()))
                    {
                        Log.Error("Cannot find extern, shutting down virtual environment...");
                        return;
                    }
                    if (Registers32bit["eax".GetHashCode()].Count != 4)
                    {
                        foreach (byte b in Registers32bit["eax".GetHashCode()])
                            Console.WriteLine(b);
                        //err
                        Log.Error("Cannot read eax value for print extern execution, shutting down virtual environment...");
                        return;
                    }
                    if (Registers32bit["ebx".GetHashCode()].Count != 4)
                    {
                        //err
                        Log.Error("Cannot read ebx value for print extern execution, shutting down virtual environment...");
                        return;
                    }
                    ConsoleColor ForegroundColor = DefineForegroundColor(Registers32bit["eax".GetHashCode()]);
                    if (ForegroundColor == ConsoleColor.Black)
                    {
                        //err
                        Log.Error("Cannot define foreground color, shutting down virtual environment...");
                        return;
                    }
                    ConsoleColor BackgroundColor = DefineBackgroundColor(Registers32bit["ebx".GetHashCode()]);
                    if (BackgroundColor == ConsoleColor.White)
                    {
                        //err
                        Log.Error("Cannot define background color, shutting down virtual environment...");
                        return;
                    }
                    string str = Encoding.ASCII.GetString(OtherRegisters["si".GetHashCode()].ToArray());
                    ConsoleColor[] old = new ConsoleColor[2] { Console.ForegroundColor, Console.BackgroundColor };
                    Console.ForegroundColor = ForegroundColor;
                    Console.BackgroundColor = BackgroundColor;
                    Console.WriteLine(str);
                    Console.ForegroundColor = old[0];
                    Console.BackgroundColor = old[1];
                    pointer++;
                }
                else if (Memory[pointer] == 0xA2)
                {
                    if (!Externs.Contains("beep".GetHashCode()))
                    {
                        //err
                        Console.WriteLine("Unknown extern");
                        return;
                    }
                    Console.Beep();
                    pointer++;
                }
                else if (Memory[pointer] == 0xB2 && Memory[pointer + 1] == 0xBB)
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
        private static void SetRegisterValue(int register, List<byte> value)
        {
            if (register == "eax".GetHashCode() || register == "ebx".GetHashCode() || register == "ecx".GetHashCode() || register == "edx".GetHashCode())
            {
                Console.WriteLine("Setting register " + register + "");
                Console.WriteLine($"{value[0]} {value[1]} {value[2]} {value[3]}");
                if (value.Count == 4)
                    Registers32bit[register.GetHashCode()] = value;
                else if (value.Count < 4 || value.Count > 0)
                {
                    Console.WriteLine("Setting " + register);
                    foreach (byte b in value)
                        Console.WriteLine(b);
                    for (int j = 0; j < value.Count; j++)
                        Registers32bit[register.GetHashCode()][j] = value[j];
                }
                else
                {
                    //err
                    Console.WriteLine("Cannot set register");
                }
            }
            else if (register == "ax".GetHashCode() || register == "bx".GetHashCode() || register == "cx".GetHashCode() || register == "dx".GetHashCode())
            {
                if (value.Count == 2)
                    Registers16bit[register.GetHashCode()] = value;
                else if (value.Count == 1)
                    Registers16bit[register.GetHashCode()][0] = value[0];
                else
                {
                    //err
                    Console.WriteLine("Cannot set register");
                }
            }
            else if (register == "si".GetHashCode())
            {
                OtherRegisters[register.GetHashCode()] = value;
            }
            else
            {
                //err
                Console.WriteLine("Cannot find register");
            }
        }
        private static void SetMemoryCellValueFromRegister(int memAddr, int registerHashCode)
        {
            if (Registers4BContains(registerHashCode))
                Memory[memAddr] = Registers32bit[registerHashCode][3];
            else if (Registers2BContains(registerHashCode))
                Memory[memAddr] = Registers16bit[registerHashCode][1];
            else
                throw new Exception("Unknown or unsupported register code!");
        }
    }
}
