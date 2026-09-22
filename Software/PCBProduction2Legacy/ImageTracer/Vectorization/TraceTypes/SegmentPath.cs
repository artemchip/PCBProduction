using System.Collections.Generic;
using ImageTracerNet.Vectorization.Segments;

namespace ImageTracerNet.Vectorization.TraceTypes
{
    public class SegmentPath
    {
        public IReadOnlyList<Segment> Segments { get; set; }
    }
}
