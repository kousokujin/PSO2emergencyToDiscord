using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PSO2emergencyToDiscord
{
    class simpleText : picture
    {
        Font fnt;
        Brush color;

        int maxHeight;
        int maxWidth;

        public simpleText(int width, int height, string filename, Font fnt, Brush color) : base(width, height, filename)
        {
            this.fnt = fnt;
            this.color = color;

            maxHeight = 0;
            maxWidth = 0;
        }

        public void drawText(string text)
        {
            Tuple<int, int> result = drawLine(text, fnt, color, 0, 0);
            maxWidth = result.Item1;
            maxHeight = result.Item2;
        }

        public override void Trimming()
        {
            Rectangle rect = new Rectangle(0, 0, img.Width, maxHeight);
            img = img.Clone(rect, img.PixelFormat);
        }

        public override void Dispose()
        {
            fnt.Dispose();
            color.Dispose();

            grp.Dispose();
            img.Dispose();
        }
    }
}
