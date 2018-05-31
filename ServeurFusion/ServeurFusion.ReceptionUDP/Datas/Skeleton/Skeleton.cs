using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ServeurFusion.ReceptionUDP.Datas
{
    public class Skeleton
    {
        // Timestamp - 8 bytes
        public long Timestamp { get; set; }
        // Tag - 1 byte
        public byte Tag { get; set; }

        public List<SkeletonPoint> SkeletonPoints { get; set; }
    }
}