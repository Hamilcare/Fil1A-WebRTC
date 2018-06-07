using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServeurFusion.ReceptionUDP.Datas.Cloud;

namespace ServeurFusion.ReceptionUDP.Datas.PointCloud
{
    /// <summary>
    /// Class who represent a cloud point.
    /// </summary>
    public class Cloud
    {
        public long Timestamp { get; set; }

        /// <summary>
        /// A list of point thats form the cloud
        /// </summary>
        public List<CloudPoint> Points { get; set; }
    }
}
