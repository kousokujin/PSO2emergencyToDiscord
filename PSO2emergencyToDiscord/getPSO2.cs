﻿using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections;

namespace PSO2emergencyToDiscord
{
    class getPSO2
    {
        //const string url = "https://akakitune87.net/api/v2/pso2ema";
        const string url = "https://akakitune87.net/api/v3/pso2ema";
        WebClient wc;
        public dynamic dataParse;
        //public List<emgPSO2Data> emgArr;
        public ArrayList emgArr;

        dialog dag;

        public getPSO2()
        {
            wc = new WebClient();
            //wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.Encoding = Encoding.UTF8;

            //緊急の情報を取得
            //reGet();

            dag = new dialog();
        }

        //再取得
        public void reGet()
        {
            //現在時刻を取得
            DateTime dt = DateTime.Now;
            //bool live = false;  //ライブフラグ
            //string liveName = "";
            int getDays = 7-((int)dt.DayOfWeek+4)%7;    //この先の緊急を取得する日数

            if (getDays == 7)   //水曜日の時
            {
                DateTime dt1630 = new DateTime(dt.Year, dt.Month, dt.Day, 17, 00, 0);   //今日の17:00
                if (DateTime.Compare(dt, dt1630) <= 0)
                {
                    getDays = 0;
                }
                /*
                else
                {
                    getDays = 0;
                }
                */
            }

            if(emgArr == null)
            {
                emgArr = new ArrayList();
            }

            if (emgArr.Count != 0) //emgArrの要素をすべて削除
            {
                emgArr.Clear();
            }

            //緊急の情報を取得
            for (int i = 0; i <= getDays; i++)
            {
                DateTime getEmgTime = dt + new TimeSpan(i, 0, 0, 0);
                string data = "\"" + getEmgTime.ToString("yyyyMMdd") + "\"";
                //string data = "\"" + "20170701" + "\"";

                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string jsonRaw = wc.UploadString(url, data);

                //パース
                dataParse = DynamicJson.Parse(jsonRaw);
                //emgArr = new List<emgPSO2Data>();
                //emgArr = new ArrayList();

                //if (dataParse != null)
                //{
                string livename = "";
                bool liveFlag = false;

                foreach (dynamic content in dataParse)
                {
                    if (content.evantType == "ライブ")  //ライブの時は情報を保存
                    {
                        livename = content.evant;
                        liveFlag = true;
                    }
                    else
                    {
                        DateTime emgDT = new DateTime(DateTime.Now.Year, (int)content.month, (int)content.date, (int)content.hour, (int)content.minute, 0);
                        evant tmp;

                        if (liveFlag == true)
                        {
                            tmp = new emgQuest(emgDT, content.evant,livename);
                            liveFlag = false;
                        }
                        else
                        {
                            
                            switch (content.evantType)
                            {
                                case "緊急クエスト":
                                    tmp = new emgQuest(emgDT, content.evant);
                                    break;
                                case "カジノイベント":
                                    tmp = new casino(emgDT);
                                    break;
                                default:
                                    tmp = new emgQuest(emgDT, "");
                                    break;

                            }
                        }
                        //emgPSO2Data tmp = new emgPSO2Data(emgDT, content.evant);
                        emgArr.Add(tmp);
                    }
                }
                //}
            }

            string logStr = "緊急クエストの情報を取得しました。取得した緊急クエストは以下の通りです。\n";
            foreach (emgPSO2Data cnt in emgArr)
            {
                logStr += string.Format("[{0}]{1}\n", cnt.time.ToString("MM/dd HH:mm"), cnt.name);
            }

            log.writeLog(logStr);
        }
    }

    class emgPSO2Data
    {
        public DateTime time;
        public string name;
        //public int evantType;

        public emgPSO2Data(DateTime t, string str)
        {
            this.time = t;
            this.name = str;
            //this.evantType = evantType;
        }
        /*

        public emgPSO2Data(DateTime t,string evantName,string evantType):this(t,evantName,type)
        {
            int type;

            switch (evantType)
            {
                case "緊急":
                    type = 1;
                    break;
                case "ライブ":
                    type = 2;
                    break;
                case "カジノイベント":
                    type = 3;
                    break;
                default:
                    type = 0;
                    break;
            }

            emgPSO2Data(t, evantName, type);
        }
        */
    }
}
