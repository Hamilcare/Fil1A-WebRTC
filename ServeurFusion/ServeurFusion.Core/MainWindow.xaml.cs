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
    /// >Interraction logic MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebRtcCommunication _webRtcSender;

        private List<UdpSkeletonListener> _kinectSkeletonList;
        private List<UdpCloudListener> _kinectCloudList;

        //private TransformationSkeletonService _skeletonTransformationService;
        //private TransformationCloudService _cloudTransformationService;

        //private BlockingCollection<Skeleton> _skeletonUdpToMiddle = new BlockingCollection<Skeleton>();
        private BlockingCollection<Skeleton> _skeletonMiddleToWebRtc = new BlockingCollection<Skeleton>();

        //private BlockingCollection<Cloud> _cloudUdpToMiddle = new BlockingCollection<Cloud>();
        private BlockingCollection<Cloud> _cloudMiddleToWebRtc = new BlockingCollection<Cloud>();

        public MainWindow()
        {
            InitializeComponent();
            _kinectSkeletonList = new List<UdpSkeletonListener>();
            _kinectCloudList = new List<UdpCloudListener>();
            AddKinectListener(9877, 9876);
        }

        private void BtnShowConsole_Click(object sender, RoutedEventArgs e)
        {
            Program.ShowConsole();
        }

        private void BtnHideConsole_Click(object sender, RoutedEventArgs e)
        {
            Program.HideConsole();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // Disable start button
            ToggleControls(true);

            //_kinectSkeletonList = new List<UdpSkeletonListener>
            //{
            //    new UdpSkeletonListener(_skeletonMiddleToWebRtc, 9877),
            //    new UdpSkeletonListener(_skeletonMiddleToWebRtc, 9887)
            //};

            //_kinectCloudList = new List<UdpCloudListener>
            //{
            //    new UdpCloudListener(_cloudMiddleToWebRtc, 9876),
            //    new UdpCloudListener(_cloudMiddleToWebRtc, 9886)
            //};

            //_skeletonTransformationService = new TransformationSkeletonService(_skeletonUdpToMiddle, _skeletonMiddleToWebRtc);
            //_cloudTransformationService = new TransformationCloudService(_cloudUdpToMiddle, _cloudMiddleToWebRtc);

            _webRtcSender = new WebRtcCommunication(_skeletonMiddleToWebRtc, _cloudMiddleToWebRtc);

            _kinectSkeletonList.ForEach(ksList => ksList.Listen());
            //_skeletonTransformationService.Start();

            _kinectCloudList.ForEach(kcList => kcList.Listen());
            //_cloudTransformationService.Start();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            ToggleControls(false);
            ListBoxPorts.Items.Clear();

            _webRtcSender.Close();

            _kinectSkeletonList.ForEach(ksList => ksList.Stop());
            _kinectSkeletonList.Clear();
            //_skeletonTransformationService.Stop();

            _kinectCloudList.ForEach(kcList => kcList.Stop());
            _kinectCloudList.Clear();
            //_cloudTransformationService.Stop();
            AddKinectListener(9877, 9876);

        }

        private void BtnAddKinect_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TxtBoxCloudPort.Text) || String.IsNullOrWhiteSpace(TxtBoxSkeletonPort.Text))
            {
                MessageBox.Show("Ports can't be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int skeletonPort, cloudPort;
            if(!Int32.TryParse(TxtBoxSkeletonPort.Text, out skeletonPort))
            {
                MessageBox.Show("Skeleton port must be integer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!Int32.TryParse(TxtBoxCloudPort.Text, out cloudPort))
            {
                MessageBox.Show("Cloud port must be integer", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int minPort = 1025;
            int maxPort = 65535;
            if(skeletonPort < minPort || skeletonPort > maxPort || cloudPort < minPort || cloudPort > maxPort)
            {
                MessageBox.Show($"Ports must be set between {minPort} and {maxPort}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AddKinectListener(skeletonPort, cloudPort);
        }

        private void AddKinectListener(int skeletonPort, int cloudPort)
        {
            _kinectCloudList.Add(new UdpCloudListener(_cloudMiddleToWebRtc, cloudPort));
            _kinectSkeletonList.Add(new UdpSkeletonListener(_skeletonMiddleToWebRtc, skeletonPort));
            ListBoxPorts.Items.Add($"Kinect : (skeleton = {skeletonPort} ; cloud = {cloudPort})");
        }

        /// <summary>
        /// Activate/Desactivate the controls in GUI
        /// </summary>
        /// <param name="serverRunning">True if the server is launching (deactivate the controls), false if the server is stoping</param>
        private void ToggleControls(bool serverRunning)
        {
            BtnStart.IsEnabled = !serverRunning;
            BtnStop.IsEnabled = serverRunning;
            BtnAddKinect.IsEnabled = !serverRunning;
            TxtBoxCloudPort.IsEnabled = !serverRunning;
            TxtBoxSkeletonPort.IsEnabled = !serverRunning;
        }


    }
}
