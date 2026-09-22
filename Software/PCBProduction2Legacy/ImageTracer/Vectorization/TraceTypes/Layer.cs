using System.Collections.Generic;

namespace ImageTracerNet.Vectorization.TraceTypes
{
    public class Layer<T> where T : class
    {
        public IReadOnlyList<T> Paths { get; set; }
    }
}
