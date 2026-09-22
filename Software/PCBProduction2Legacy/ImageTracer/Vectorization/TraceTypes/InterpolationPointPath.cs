using System.Collections.Generic;
using ImageTracerNet.Vectorization.Points;

namespace ImageTracerNet.Vectorization.TraceTypes
{
    public class InterpolationPointPath
    {
        public IReadOnlyList<InterpolationPoint> Points { get; set; }
    }
}
