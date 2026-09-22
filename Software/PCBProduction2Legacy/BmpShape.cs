using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction2
{
    public class BmpShape
    {
        public List<Point> allPoints = new List<Point>();
        public Point centerPoint = Point.Empty;

        public BmpShape(List<Point> allPoints, Point centerPoint)
        {
            this.allPoints = allPoints;
            this.centerPoint = centerPoint;
        }
    }
}
