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
        public long Tag { get; set; }
        public IList<CloudPoint> Points { get; set; }
    }
}
