using System;
using System.Collections.Concurrent;
using System.Windows;
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
        private WebRtcCommunication _webRtcSender;

        private UdpSkeletonListener _skeletonUdpListener;
        private UdpCloudPointListener _cloudUdpListener;
        private TransformationSkeletonService _skeletonTransformationService;
        private TransformationCloudPointService _cloudTransformationService;

        //Structures de données 
        private BlockingCollection<Skeleton> _skeletonUdpToMiddle = new BlockingCollection<Skeleton>();
        private BlockingCollection<Skeleton> _skeletonMiddleToWebRtc = new BlockingCollection<Skeleton>();

        private BlockingCollection<Cloud> _cloudUdpToMiddle = new BlockingCollection<Cloud>();
        private BlockingCollection<Cloud> _cloudMiddleToWebRtc = new BlockingCollection<Cloud>();


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

            _skeletonUdpListener = new UdpSkeletonListener(_skeletonUdpToMiddle, 9877);
            _cloudUdpListener = new UdpCloudPointListener(_cloudUdpToMiddle, 9876);
            _skeletonTransformationService = new TransformationSkeletonService(_skeletonUdpToMiddle, _skeletonMiddleToWebRtc);
            _cloudTransformationService = new TransformationCloudPointService(_cloudUdpToMiddle, _cloudMiddleToWebRtc);

            _webRtcSender = new WebRtcCommunication(_skeletonMiddleToWebRtc, _cloudMiddleToWebRtc);

            _skeletonUdpListener.Listen();
            _skeletonTransformationService.Start();

            _cloudUdpListener.Listen();
            _cloudTransformationService.Start();

            _webRtcSender.Connect();
            //Console.ReadLine();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;

            _webRtcSender.Close();

            _skeletonUdpListener.Stop();
            _skeletonTransformationService.Stop();

            _cloudUdpListener.Stop();
            _cloudTransformationService.Stop();
        }
    }
}
