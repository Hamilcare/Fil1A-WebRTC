using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using ServeurFusion.ReceptionUDP.UdpListeners;
using ServeurFusion.ReceptionUDP.TransformationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServeurFusion.Core
{
    
    class Program
    {
        #region console logic
        //Cette région comporte la logique d'affichage de la console (permet d'afficher et de masquer la console sur l'appli WPF
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole(); // Create console window

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow(); // Get console window handle

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SW_SHOW);
        }

        public static void HideConsole()
        {
            var handle = GetConsoleWindow();
            if (handle != null)
                ShowWindow(handle, SW_HIDE);
        }
        #endregion

        public static Application WinApp { get; private set; }
        public static Window MainWindow { get; private set; }
        

        static void InitializeWindows()
        {
            WinApp = new Application();
            WinApp.Run(MainWindow = new MainWindow()); // note: blocking call
        }

        [STAThread]    
        static void Main(string[] args)
        {
            //Launch GUI
            InitializeWindows(); // opens the WPF window and waits here
        }
    }

}
