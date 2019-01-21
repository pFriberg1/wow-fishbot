using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;

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
        private static bool loopFlag = false;
        public enum States
        {
            Starting, Waiting, Looting, Debug
        }

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        /*[DllImport("gdi32.dll", SetLastError = true)]
        static extern uint GetPixel(IntPtr dc, int x, int y);

        
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

        private static Color GetColorAt(Point pos, Bitmap bmp)
        {

            return bmp.GetPixel(pos.X, pos.Y);
           
        }

        private static Bitmap GetBitmap()
        {
            var bmpScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var graphics = Graphics.FromImage(bmpScreen);
            graphics.CopyFromScreen(0, 0, 0, 0, bmpScreen.Size);
            bmpScreen.Save("img.png", ImageFormat.Png);

            return bmpScreen;
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

        private static void SetLoopFlag(bool status)
        {
            loopFlag = status;
        }

        private static bool GetLoopFlag()
        {
            return loopFlag;
        }

        //Finds the bobber and moves the mouse to the bobber, returns the position the bobber was found, if no target was found returns the position 0,0
        private static Point FindTarget(Bitmap bmp, Color target)
        {
            const int targetOffset = 15;
          
            var widthLower = bmp.Width / 4;
            var widthHigher = (bmp.Width / 4) * 3;
            var heightLower = bmp.Height / 4;
            var heightHigher = (bmp.Height / 4) * 3;

            var targetRedLb = target.R - targetOffset;
            var targetRedHb = target.R + targetOffset;
            var targetBlueLb = target.B - targetOffset;
            var targetBlueHb = target.B + targetOffset;
            var targetGreenLb = target.G - targetOffset;
            var targetGreenHb = target.G + targetOffset;

            var pos = new Point(0, 0);

            for (int i = widthLower; i < widthHigher; i++)
            {
                for (int j = heightLower; j < heightHigher; j++)
                {
                    pos.X = i;
                    pos.Y = j;
                    if (GetColorAt(pos, bmp).R > targetRedLb && GetColorAt(pos, bmp).R < targetRedHb && GetColorAt(pos, bmp).B > targetBlueLb 
                        && GetColorAt(pos, bmp).B < targetBlueHb && GetColorAt(pos, bmp).G > targetGreenLb && GetColorAt(pos, bmp).G < targetGreenHb)
                    {
                        //Console.WriteLine("Target found at position: " + pos);
                        if (!GetLoopFlag())
                        {
                            MoveMouse(pos);
                            SetLoopFlag(true);
                        }
                        Thread.Sleep(50);
                        return pos;
                    }
                }
            }

            return Point.Empty;
        }

        private static void CastLine()
        {
            var r = new Random();
            var rInt = r.Next(0, 75);
            

            PostMessage(GetWoWProcess().MainWindowHandle, WM_KEYDOWN, VK_KEY_4, 0);
            Thread.Sleep(50 + rInt);
            PostMessage(GetWoWProcess().MainWindowHandle, WM_KEYUP, VK_KEY_4, 0);
        }

        private static void Loot()
        {
            var r = new Random();
            var rInt = r.Next(0, 47);

            PostMessage(GetWoWProcess().MainWindowHandle, WM_LBUTTONDOWN, VK_RMB, 0);
            Thread.Sleep(30 + rInt);
            PostMessage(GetWoWProcess().MainWindowHandle, WM_LBUTTONUP, VK_RMB, 0);
        }

        private static bool WaitForFish(Color targetColor)
        {
            while (FindTarget(GetBitmap(), targetColor).X == 0 && FindTarget(GetBitmap(), targetColor).Y == 0)
            {
                
            }
            var startPos = FindTarget(GetBitmap(), targetColor);
            Console.WriteLine("Start pos: " + startPos);
            var timer = new Stopwatch();
            timer.Start();
            while (timer.Elapsed.TotalSeconds < 25)
            {
                var deltaPos = FindTarget(GetBitmap(), targetColor);
                Console.WriteLine("Delta pos: " + deltaPos);
                if (deltaPos.Y >= startPos.Y + 6 && !deltaPos.IsEmpty)
                {
                    return true;
                }
            }
            timer.Stop();
            return false;
        }


       
        private static void FishingBot(Color targetColor)
        {
            var r = new Random();
            var rInt = r.Next(1000, 2500);
            var currentState = States.Starting;
            while (true)
            {
                switch (currentState)
                {
                    case (States.Starting):
                        CastLine();
                        SetLoopFlag(false);
                        currentState = States.Waiting;
                        break;
                    case (States.Waiting):
                        if (WaitForFish(targetColor))
                        {
                            currentState = States.Looting;
                        }
                        else
                        {
                            currentState = States.Starting;
                        }
                        break;
                    case (States.Looting):
                        Loot();
                        SetLoopFlag(false);
                        Thread.Sleep(2000 + rInt);
                        currentState = States.Starting;
                        break;
                    case (States.Debug):
                        Console.WriteLine("One state loop completed!");
                        var keyInput = Console.ReadKey();
                        if (keyInput.Key == ConsoleKey.A)
                        {
                            currentState = States.Starting;
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
        }

      
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Press 'B' to enter your fishing key bind.");
            Console.WriteLine("Press 'C' to calibrate the targeting algorithm.");
            Console.WriteLine("Press 'S' to start the bot.");
            Console.WriteLine("Pressing 'ESC' will always terminate the program.");

            var targetColor = new Color();

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
                        Console.WriteLine("2. Hover over the red feather of the bobber");
                        Console.WriteLine("3. Press 'F' to calibrate");
                        keyInput = Console.ReadKey();
                        if (keyInput.Key == ConsoleKey.F)
                        {
                            targetColor = GetColorAt(GetMousePosition(), GetBitmap());
                            Console.WriteLine("\nTarget {0}, PosX {1}, PosY{2}", targetColor, GetMousePosition().X, GetMousePosition().Y);
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
                        Thread.Sleep(3000);
                        FishingBot(targetColor);
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine("\nEscape key pressed");
                        Environment.Exit(0);
                        break;

                }
            }

        }
    }
}
