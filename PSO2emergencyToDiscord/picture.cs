using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PSO2emergencyToDiscord
{
    abstract class picture
    {
        public string filename;
        public Bitmap img;
        public Graphics grp;

        public picture(int width,int height,string name)
        {
            img = new Bitmap(width, height);
            grp = Graphics.FromImage(img);
            filename = name;
        }

        public void saveImage() //イメージをファイルに保存
        {
            img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        virtual public void imageClear()    //イメージのクリア
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
        }

        public SizeF measureFont(Font fnt,string str)
        {
            StringFormat sf = new StringFormat();
            SizeF stringSize = grp.MeasureString(str, fnt, 10000, sf);

            return stringSize;
        }

        public Tuple<int,int> drawLine(string str, Font fnt,Brush fontColor,int x,int y) //Tupleは幅、高さの順
        {
            string dStr = str;
            string testStr = "";
            int drawCount = 0;

            int maxHeight = 0;
            int maxWidth = 0;

            for (int i = 0; i < str.Length;)
            {
                testStr += str[i];
                if (str[i] == '\n' || img.Width < (measureFont(fnt, testStr).Width + x))
                {
                    char privent = str[i];
                    testStr = testStr.Remove(testStr.Length - 1, 1);
                    grp.DrawString(testStr, fnt, fontColor, x, y + maxHeight);
                    maxHeight += fnt.Height;
                    float fntWidth = measureFont(fnt, testStr).Width;

                    if (maxWidth < fntWidth)
                    {
                        maxWidth = (int)fntWidth;
                    }

                    if (privent == '\n')
                    {
                        dStr.Remove(0, drawCount + 1);
                        i++;
                    }
                    else
                    {
                        dStr.Remove(0, drawCount);
                    }
                    testStr = "";
                    drawCount = 0;
                }
                else
                {
                    i++;
                    drawCount++;
                    if (i == str.Length)
                    {
                        grp.DrawString(testStr, fnt, fontColor, x, y + maxHeight);
                        maxHeight += fnt.Height;
                        float fntWidth = measureFont(fnt, testStr).Width;
                        if (maxWidth < fntWidth)
                        {
                            maxWidth = (int)fntWidth;
                        }
                    }
                }
            }

            return new Tuple<int, int>(maxWidth, maxHeight);
        }

        abstract public void Trimming();
    }
}
