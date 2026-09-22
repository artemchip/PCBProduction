using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction2
{
    public class ALVertice
    {
        public float locXMm = 0f;
        public float locYMm = 0f;
        public float zTouchLevelMm = 0f;

        public ALVertice(float locXMm, float locYMm, float zTouchLevelMm)
        {
            this.locXMm = locXMm;
            this.locYMm = locYMm;
            this.zTouchLevelMm = zTouchLevelMm;
        }
    }
}
