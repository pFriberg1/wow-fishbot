using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

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

        [STAThread]
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Press 'B' to enter your fishing keybind.");
                Console.WriteLine("Press 'C' to calibrate the targeting algortihm.");
                Console.WriteLine("Press 'S' to start the bot.");
                Console.WriteLine("Pressing 'ESC' will always terminate the program.");
                var input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.B:
                        Console.WriteLine("Enter the keybind for your fishing skill.");
                        var keybind = Console.ReadKey();
                        Console.WriteLine(keybind.Key.ToString() + " has been saved as your keybind.");
                        break;
                    case ConsoleKey.C:
                        break;
                    case ConsoleKey.S:
                        break;
                    case ConsoleKey.Escape:
                        Application.Exit();
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

        private static Point GetMousePosition()
        {
            return Cursor.Position;
        }
        private static void MoveMouse(Point position)
        {
            Cursor.Position = position;
        }
    }
}
