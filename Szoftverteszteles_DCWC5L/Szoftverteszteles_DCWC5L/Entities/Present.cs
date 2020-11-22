using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szoftverteszteles_DCWC5L.Abstractions;

namespace Szoftverteszteles_DCWC5L.Entities
{
    public class Present : Toy
    {
        public SolidBrush RibbonColor { get; private set; }

        public SolidBrush BoxColor { get; private set; }

        public Present(Color ribbon, Color box)
        {
            RibbonColor = new SolidBrush(ribbon);
            BoxColor = new SolidBrush(box);
        }
        
        protected override void DrawImage(Graphics g)
        {
            g.FillRectangle(BoxColor, 0, 0, Width, Height);
        }
    }
}
