using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PSO2emergencyToDiscord
{
    class todayEventImage : picture
    {
        //ArrayList evn;
        public Font fnt;
        public Brush fontColor;

        //各種パラメータ
        int timeWidth;  //時刻を表示する幅
        int nameWidth;  //緊急名を表示する幅
        int timenameMargin; //時刻と緊急名の間の空白

        //描画用のパラメータ
        private int drawHeight;
        private int maxWidth;

        public todayEventImage(int width, int height, string filename,Font fnt, Brush color) : base(width, height,filename)
        {
            //this.evn = env;
            this.fnt = fnt;
            fontColor = color;
            drawHeight = 0;
            maxWidth = 0;
            setParameter();
        }

        public void setParameter(int timeWidth = 100, int nameWidth = 500, int timenameMargin = 5)
        {
            this.timeWidth = timeWidth;
            this.nameWidth = nameWidth;
            this.timenameMargin = timenameMargin;
        }

        public override void Trimming()
        {
            //int width = img.Width;
            Rectangle rect = new Rectangle(0, 0, maxWidth, drawHeight);
            img = img.Clone(rect, img.PixelFormat);
        }

        public override void imageClear()
        {
            if (img != null)
            {
                int width = img.Width;
                int height = img.Height;
                img.Dispose();
                grp.Dispose();
                img = new Bitmap(width, height);
                grp = Graphics.FromImage(img);
            }

            drawHeight = 0;
        }

        public void drawString(string str,int margin = 0)
        {
            Tuple<int,int> result = drawLine(str, fnt, fontColor, margin, drawHeight);
            if(result.Item1 > maxWidth)
            {
                maxWidth = result.Item1;
            }
            drawHeight += result.Item2;
            
        }

        public void drawOneline(string time,string name)
        {
            int x = 0;
            grp.DrawString(time, fnt, fontColor, x, drawHeight);
            x += (timeWidth + timenameMargin);
            //grp.DrawString(name, fnt, fontColor, x, drawHeight);
            Tuple<int,int> result = drawLine(name, fnt,fontColor,x,drawHeight);
            drawHeight += result.Item2;

            if (maxWidth < (x+result.Item1))
            {
                maxWidth = x + result.Item1;
            }
        }

    }
}
