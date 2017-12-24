using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    abstract class Event
    {
        public DateTime eventTime;
        public string eventName;

        public Event(DateTime time,string evant)
        {
            this.eventTime = time;
            this.eventName = evant;
        }
    }

    class emgQuest : Event  //緊急クエスト
    {
        /*
        public string live
        {
            get
            {
                return live;
            }
            set
            {
                //liveEnable = (value != "");
                if (value != "")
                {
                    liveEnable = true;
                }
                else
                {
                    liveEnable = false;
                }
                live = value;
            }
        }
        */

        public string live;
        public bool liveEnable;

        public emgQuest(DateTime time, string evant) : base(time, evant)
        {
            liveEnable = false;
            live = "";
        }

        public emgQuest(DateTime time,string evant,string liveName) : base(time, evant)
        {
            live = liveName;
            liveEnable = true;
        }
    }

    class casino : Event    //カジノイベント
    {
        public casino(DateTime time) : base(time,"カジノイベント"){

        }
    }
}
