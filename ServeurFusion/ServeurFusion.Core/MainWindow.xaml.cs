using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServeurFusion.EnvoiRTC;
using ServeurFusion.ReceptionUDP;
using ServeurFusion.ReceptionUDP.Datas;
using ServeurFusion.ReceptionUDP.Datas.PointCloud;
using ServeurFusion.ReceptionUDP.TransformationServices;
using ServeurFusion.ReceptionUDP.UdpListeners;

namespace ServeurFusion.Core
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Structures de données 
        private DataTransferer<Skeleton> skeletonUdpToMiddle = new DataTransferer<Skeleton>();
        private DataTransferer<Skeleton> skeletonMiddleToWebRtc = new DataTransferer<Skeleton>();

        private DataTransferer<Cloud> cloudUdpToMiddle = new DataTransferer<Cloud>();
        private DataTransferer<Cloud> cloudMiddleToWebRtc = new DataTransferer<Cloud>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnShowConsole_Click(object sender, RoutedEventArgs e)
        {
            Program.ShowConsole();
        }

        private void BtnHideConsole_Click(object sender, RoutedEventArgs e)
        {
            Program.HideConsole();
        }

        private void BtnCustomDebug_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("hello world !");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            //Désactive le bouton start
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;

            var skeletonUdpListener = new UdpSkeletonListener(skeletonUdpToMiddle, 9877);
            var cloudUdpListener = new UdpCloudPointListener(cloudUdpToMiddle, 9876);
            var skeletonTransformationService = new TransformationSkeletonService(skeletonUdpToMiddle, skeletonMiddleToWebRtc);
            var cloudTransformationService = new TransformationCloudPointService(cloudUdpToMiddle, cloudMiddleToWebRtc);

            WebRtcCommunication webRtcSender = new WebRtcCommunication(skeletonMiddleToWebRtc, cloudMiddleToWebRtc);

            skeletonUdpListener.Listen();
            skeletonTransformationService.Prosecute();

            cloudUdpListener.Listen();
            cloudTransformationService.Prosecute();

            webRtcSender.Connect();
            //Console.ReadLine();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;
        }
    }
}
