using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction2
{
    public class MotionCalibratePoint
    {
        public PointF machinePosition = new PointF(0.0f, 0.0f);
        public List<PointF> patternPoints = new List<PointF>();

        public PointF GetPatternCenter()
        {
            float sumX = 0.0f;
            float sumY = 0.0f;
            foreach (PointF p in patternPoints) {
                sumX += p.X;
                sumY += p.Y;
            }        
            sumX /= patternPoints.Count;
            sumY /= patternPoints.Count;

            return new PointF(sumX, sumY);
        }
    }
}
