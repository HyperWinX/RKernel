using Cosmos.HAL.Drivers.Audio;
using Cosmos.HAL.Drivers.Video.SVGAII;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKernel.GUIEngine
{
    internal static class MainGUI
    {
        private static SVGAIICanvas canvas;
        private static Image wallpaper;
        public static void InitializeGUI()
        {
            canvas = new(new Mode(1280, 720, ColorDepth.ColorDepth24));
            Console.WriteLine("Driver initialized...");
            wallpaper = new Bitmap(File.ReadAllBytes("0:\\RKernel\\data\\wallpaper.bmp"));
            Console.WriteLine("Wallpaper loaded...");
        }
        private static void Run()
        {
            canvas.Display();
            while (true)
            {
                canvas.DrawImage(wallpaper, 0, 0);

            }
        }
    }
}
