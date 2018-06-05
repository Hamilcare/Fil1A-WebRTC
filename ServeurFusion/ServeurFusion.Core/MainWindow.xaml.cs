using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private List<UdpSkeletonListener> _kinectSkeletonList;
        private List<UdpCloudListener> _kinectCloudList;

        private TransformationSkeletonService _skeletonTransformationService;
        private TransformationCloudService _cloudTransformationService;

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
            // Disable start button
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;

            _kinectSkeletonList = new List<UdpSkeletonListener>
            {
                new UdpSkeletonListener(_skeletonUdpToMiddle, 9877),
                new UdpSkeletonListener(_skeletonUdpToMiddle, 9887)
            };

            _kinectCloudList = new List<UdpCloudListener>
            {
                new UdpCloudListener(_cloudUdpToMiddle, 9876),
                new UdpCloudListener(_cloudUdpToMiddle, 9886)
            };

            _skeletonTransformationService = new TransformationSkeletonService(_skeletonUdpToMiddle, _skeletonMiddleToWebRtc);
            _cloudTransformationService = new TransformationCloudService(_cloudUdpToMiddle, _cloudMiddleToWebRtc);

            _webRtcSender = new WebRtcCommunication(_skeletonMiddleToWebRtc, _cloudMiddleToWebRtc);

            _kinectSkeletonList.ForEach(ksList => ksList.Listen());
            _skeletonTransformationService.Start();

            _kinectCloudList.ForEach(kcList => kcList.Listen());
            _cloudTransformationService.Start();

            _webRtcSender.Connect();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;

            _webRtcSender.Close();

            _kinectSkeletonList.ForEach(ksList => ksList.Stop());
            _skeletonTransformationService.Stop();

            _kinectCloudList.ForEach(kcList => kcList.Stop());
            _cloudTransformationService.Stop();
        }
    }
}
