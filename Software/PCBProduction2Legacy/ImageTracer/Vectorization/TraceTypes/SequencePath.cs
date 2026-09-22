using System.Collections.Generic;

namespace ImageTracerNet.Vectorization.TraceTypes
{
    public class SequencePath
    {
        public InterpolationPointPath Path { get; set; }
        public IReadOnlyList<SequenceIndices> Sequences { get; set; }
    }
}
