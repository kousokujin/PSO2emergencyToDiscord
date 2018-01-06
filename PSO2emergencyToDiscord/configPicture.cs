using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    [Serializable]
    public class configPicture
    {
        public string fontname;
        public int fontsize;

        //フォントの色
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public int width;
        public int field1; //本日の緊急一覧の時刻フィールドの幅
        public int field2; //次の緊急までの残り時間フィールドの幅
        public int field3; //次の緊急の開始時間フィールドの幅

        public bool enabled;

        public configPicture() { }
    }
}
