using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.EnvoiRTC
{
    class SkeletonPoint
    {
        // X point - 4 bytes
        public float X { get; set; }
        // Y point - 4 bytes
        public float Y { get; set; }
        // Z point - 4 bytes
        public float Z { get; set; }

        // R color - 1 byte
        public byte R { get; set; }
        // G color - 1 byte
        public byte G { get; set; }
        // B color - 1 byte
        public byte B { get; set; }

        // Tag vector - 1 byte
        public byte Tag { get; set; }

        /*public SkeletonPoint(float x, float y, float z, byte r, byte g, byte b, byte tag)
        {
            X = x;
            Y = y;
            Z = z;
            R = r;
            G = g;
            B = b;
            Tag = tag;
        }*/
    }
}
