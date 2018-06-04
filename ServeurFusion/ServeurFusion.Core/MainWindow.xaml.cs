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
        private WebRtcCommunication _webRtcSender;

        private UdpSkeletonListener _skeletonUdpListener;
        private UdpCloudPointListener _cloudUdpListener;
        private TransformationSkeletonService _skeletonTransformationService;
        private TransformationCloudPointService _cloudTransformationService;

        //Structures de données 
        private DataTransferer<Skeleton> _skeletonUdpToMiddle = new DataTransferer<Skeleton>();
        private DataTransferer<Skeleton> _skeletonMiddleToWebRtc = new DataTransferer<Skeleton>();

        private DataTransferer<Cloud> _cloudUdpToMiddle = new DataTransferer<Cloud>();
        private DataTransferer<Cloud> _cloudMiddleToWebRtc = new DataTransferer<Cloud>();


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
