using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace FisherMan
{
    class Program
    {
        private const UInt32 WM_KEYDOWN = 0x0100;
        private const UInt32 WM_KEYUP = 0x0101;
        private const UInt32 WM_LBUTTONDOWN = 0x201;
        private const UInt32 WM_LBUTTONUP = 0x202;

        private const int VK_KEY_4 = 0x34;
        private const int VK_RMB = 0x02;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern uint GetPixel(IntPtr dc, int x, int y);

        /*
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        */
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        

        //Get the Point(X, Y) for the cursors position
        private static Point GetMousePosition()
        {
            return Cursor.Position;
        }
        //Set the Cursors position to a new position
        private static void MoveMouse(Point position)
        {
            Cursor.Position = position;
        }

        private static Color GetColorAt(Point pos)
        {
            var bmpScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            var gfxScreen = Graphics.FromImage(bmpScreen);

            using (Graphics gdest = Graphics.FromImage(bmpScreen))
            {
                using (Graphics gsrc = Graphics.FromHwnd(GetWoWProcess().MainWindowHandle))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDc = gdest.GetHdc();
                    int retval = BitBlt(hDc, 0, 0, 1, 1, hSrcDC, pos.X, pos.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return bmpScreen.GetPixel(0, 0);
           
        }

        //Get the wow-process, if success returns the process else null
        private static Process GetWoWProcess()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (p.ProcessName == "Wow" ||p.ProcessName == "Wow-64")
                {
                    return p;
                }
            }
            return null;
        }



        private static void ScreenSweep()
        {
            int heightLower = Screen.PrimaryScreen.Bounds.Height/4;
            int heightHigher = heightLower * 3;

            int widthLower = Screen.PrimaryScreen.Bounds.Width/4;
            int widthHigher = widthLower * 3;
            Point mp = new Point(0, 0);

            for (int x = widthLower; x < widthHigher; x++)
            {
                for (int y = heightLower; y < heightHigher; y++)
                {
                    mp.X = x;
                    mp.Y = y;
                    Console.WriteLine("Color {0}, PosX{1}, PosY{2}",GetColorAt(mp), mp.X, mp.Y);

                    //MoveMouse(mp);
                }
            }

        }


        [STAThread]
        static void Main()
        {
            Console.WriteLine("Press 'B' to enter your fishing keybind.");
            Console.WriteLine("Press 'C' to calibrate the targeting algortihm.");
            Console.WriteLine("Press 'S' to start the bot.");
            Console.WriteLine("Pressing 'ESC' will always terminate the program.");
            Process wowProcess;
            
            while (true)
            {
               
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.B:
                        Console.WriteLine("\nEnter the keybind for your fishing skill.");
                        var keyInput = Console.ReadKey();
                        //TODO: Lookup hexcode for keybind and save it to be used for casting 
                        Console.WriteLine("\n" + keyInput.Key.ToString() + " has been saved as your keybind.");
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("\nTo calibrate the algortihm do the following steps:");
                        Console.WriteLine("1. Manually cast your bobber");
                        Console.WriteLine("2. Hower over the red feather of the bobber");
                        Console.WriteLine("3. Press 'F' to calibrate");
                        keyInput = Console.ReadKey();
                        if (keyInput.Key == ConsoleKey.F)
                        {
                            var targetColor = GetColorAt(GetMousePosition());
                            Console.WriteLine("Target color: " + targetColor);
                            //TODO: Get pixel color from cursor point and scan screen for the first pixel with that color
                            //      Then wait until that pixel moves, if it moves a fish is on the line
                            Console.WriteLine("\nAlgorithm has been calculated, pressing 'S' will start the bot.");
                        }
                        else
                        {
                            Console.WriteLine("\nPress 'C' to restart the calibration process.");
                        }
                        break;
                    case ConsoleKey.S:
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine("\nEscpae key pressed");
                        Environment.Exit(0);
                        break;

                }

                Process[] processList = Process.GetProcesses();
                foreach (Process p in processList)
                {
                    if (p.ProcessName == "Wow" || p.ProcessName == "WorldOfWarcraft" || p.ProcessName == "WoW")
                    {
                        /*
                        PostMessage(p.MainWindowHandle, WM_KEYDOWN, VK_KEY_4, 0);
                        Console.WriteLine("Key pressed!");
                        PostMessage(p.MainWindowHandle, WM_KEYUP, VK_KEY_4, 0);
                        Console.WriteLine("Key released!"); 
                        Thread.Sleep(5000);
                        PostMessage(p.MainWindowHandle, WM_LBUTTONDOWN, VK_RMB, 0);
                        Thread.Sleep(50);
                        PostMessage(p.MainWindowHandle, WM_LBUTTONUP, VK_RMB, 0);
                        Console.WriteLine("Before sleep");
                        Thread.Sleep(2000);
                        Console.WriteLine("Awoken");
                        //Thread.Sleep(20000); //Almost whole cast time for fishing
                        */

                    }
                }

            }

        }
    }
}
