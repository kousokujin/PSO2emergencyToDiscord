using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    abstract class evant
    {
        public DateTime evantTime;
        public string evantName;

        public evant(DateTime time,string evant)
        {
            this.evantTime = time;
            this.evantName = evant;
        }
    }

    class emgQuest : evant  //緊急クエスト
    {
        public string live
        {
            get
            {
                return live;
            }
            set
            {
                liveEnable = (value != "");
                live = value;
            }
        }
        
        bool liveEnable;

        public emgQuest(DateTime time, string evant) : base(time, evant)
        {
            liveEnable = false;
            live = "";
        }

        public emgQuest(DateTime time,string evant,string liveName) : base(time, evant)
        {
            live = liveName;
        }
    }

    class casino : evant    //カジノイベント
    {
        public casino(DateTime time) : base(time,"カジノイベント"){

        }
    }
}
