using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServeurFusion.ReceptionUDP.Datas.Cloud;

namespace ServeurFusion.ReceptionUDP.Datas.PointCloud
{
    public class Cloud
    {
        public long Timestamp { get; set; }
        public List<CloudPoint> Points { get; set; }
    }
}
