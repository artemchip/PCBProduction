using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class QCNet
    {
        public List<List<Point>> trackByLayer = new List<List<Point>>();
        public List<Point> viaCenters = new List<Point>();
        public List<Point> topPadCenters = new List<Point>();
        public List<Point> bottomPadCenters = new List<Point>();
    }
}
