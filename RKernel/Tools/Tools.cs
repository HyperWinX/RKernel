using System;

namespace RKernel.Tools
{
    public static class Tools
    {
        public static bool TryParse(string str, out int num)
        {
            num = 0;
            try
            {
                num = Convert.ToInt32(str);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static void ClearLines(int lines)
        {
            System.Console.SetCursorPosition(0, System.Console.CursorTop - lines);
            for (int i = 0; i < lines; i++)
                System.Console.WriteLine(' ' * System.Console.WindowWidth);
        }
        public static bool IsNumberInRange(int num, int first, int second) => (first <= num && num <= second) ? true : false;
        public static void ClearSymbols(int symbols)
        {
            System.Console.SetCursorPosition(System.Console.CursorLeft - symbols, System.Console.CursorTop);
            System.Console.Write(' ' * symbols);
            System.Console.SetCursorPosition(System.Console.CursorLeft - symbols, System.Console.CursorTop);
        }
        public static void ChangeConsoleColors(ConsoleColor foreground, ConsoleColor background)
        {
            System.Console.ForegroundColor = foreground;
            System.Console.BackgroundColor = background;
        }
        public static string RemoveSpaces(string str)
        {
            while (str.Contains(' '))
                str = str.Remove(str.IndexOf(' '), 1);
            return str;
        }
    }
}
