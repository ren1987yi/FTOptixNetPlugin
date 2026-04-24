    using System.Runtime.InteropServices;
namespace DesktopAuto
{
    class MouseSimulator
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        public static void ClickLeftButton(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        public static void LeftButtonDown()
        {

        }

        public static void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }

    
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
    }
    class KeyboardSimulator
    {

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        private const uint KEYEVENTF_KEYUP = 0x0002;
        public static void PressKey(byte keyCode)
        {
            keybd_event(keyCode, 0, 0, 0); // 按下按键
            keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0); // 松开按键
        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        
            MouseSimulator.ClickLeftButton(100, 100); // 在屏幕坐标 (100, 100) 处模拟鼠标左键点击
            Task.Delay(4000).Wait();
            MouseSimulator.ClickLeftButton(200, 200); // 在屏幕坐标 (100, 100) 处模拟鼠标左键点击
            Task.Delay(4000).Wait();
            MouseSimulator.ClickLeftButton(300, 300); // 在屏幕坐标 (100, 100) 处模拟鼠标左键点击
            Task.Delay(4000).Wait();
            MouseSimulator.ClickLeftButton(400, 400); // 在屏幕坐标 (100, 100) 处模拟鼠标左键点击
            Task.Delay(4000).Wait();

            KeyboardSimulator.PressKey(122);

        }
    }
}
