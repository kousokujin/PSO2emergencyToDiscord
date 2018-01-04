using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PSO2emergencyToDiscord
{
    class eventImage : picture
    {
        Font fnt;
        Brush color;

        //各種パラメータ
        int titleWidth;
        int titleTimemargin;
        int timeWidth;
        int timeContentmargin;
        int content;

        int maxHeight;
        int maxWidth;


        public eventImage(int width,int height,string filename,Font f,Brush c) : base(width, height, filename)
        {
            fnt = f;
            color = c;
            setParameter();
            maxHeight = 0;
            maxWidth = 0;

        }

        public void setParameter(int titleWidth = 140,int margin1=5,int timeWidth=80,int margin2=10,int contentWidth=200)
        {
            this.titleWidth = titleWidth;
            this.titleTimemargin = margin1;
            this.timeWidth = timeWidth;
            this.timeContentmargin = margin2;
            this.content = contentWidth;
        }

        public void drawText(string interval,string time,string name)
        {
            int x = 0;
            grp.DrawString(interval, fnt, color, x, 0);
            newHeight(fnt, interval);
            x += (titleWidth + titleTimemargin);
            grp.DrawString(time, fnt, color, x, 0);
            newHeight(fnt, time);
            x += (timeWidth + timeContentmargin);
            Tuple<int,int> nameResult = drawLine(name, fnt,color, x, 0);
            maxWidth = x + nameResult.Item1;

            if(maxHeight < nameResult.Item2)
            {
                maxHeight = nameResult.Item2;
            }

        }

        public override void Trimming()
        {
            //int width = img.Width;
            Rectangle rect = new Rectangle(0, 0, maxWidth, maxHeight);
            img = img.Clone(rect, img.PixelFormat);
        }


        private void newHeight(Font fnt,string str)
        {
            int fontHeight = (int)measureFont(fnt, str).Height;
            if(this.maxHeight < fontHeight)
            {
                this.maxHeight = fontHeight;
            }
        }
    }
}
