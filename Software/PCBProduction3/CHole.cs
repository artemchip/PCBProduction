using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class CHole
    {
        public bool isAnchor = false;
        public float holeCenterMmX = 0.0f;
        public float holeCenterMmY = 0.0f;
        public float diameterMm = 0.0f;
        public bool fromPartialPCB = false;

        public CHole(bool isAnchor, float holeCenterX, float holeCenterY, float diameter, bool fromPartialPCB)
        {
            this.isAnchor = isAnchor;
            this.holeCenterMmX = holeCenterX;
            this.holeCenterMmY = holeCenterY;
            this.diameterMm = diameter;
            this.fromPartialPCB = fromPartialPCB;
        }
    }
}
