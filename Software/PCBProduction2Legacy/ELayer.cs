using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction2
{
    public class ELayer
    {
        public String fileName = "";
        public bool isCopperOuter = false;
        public bool isCopperInner = false;
        public int copperIdx = 0;
        public int totalIdx = 0;
        public bool shouldMirror = false;
        public Bitmap? bitmap;

        public ELayer(String fileName, bool isCopperOuter, bool isCopperInner, int copperIdx, int totalIdx, bool shouldMirror)
        {
            this.fileName = fileName;
            this.isCopperOuter = isCopperOuter;
            this.isCopperInner = isCopperInner;
            this.copperIdx = copperIdx;
            this.totalIdx = totalIdx;
            this.shouldMirror = shouldMirror;
            this.bitmap = null;
        }
    }
}
