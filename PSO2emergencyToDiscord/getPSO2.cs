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
        //const string url = "https://akakitune87.net/api/v3/pso2ema";
        const string url = "https://akakitune87.net/api/v4/pso2emergency";  //V4対応
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
                //string data = "\"" + getEmgTime.ToString("yyyyMMdd") + "\"";
                string data = string.Format("{{\"EventDate\":\"{0}\"}}",getEmgTime.ToString("yyyyMMdd"));
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
                    if (content.EventType == "ライブ")  //ライブの時は情報を保存
                    {
                        livename = content.EventName;
                        liveFlag = true;
                    }
                    else
                    {
                        DateTime emgDT = new DateTime(DateTime.Now.Year, (int)content.Month, (int)content.Date, (int)content.Hour, (int)content.Minute, 0);
                        Event tmp;

                        if (liveFlag == true)
                        {
                            tmp = new emgQuest(emgDT, content.EventName,livename);
                            liveFlag = false;
                        }
                        else
                        {
                            
                            switch (content.EventType)
                            {
                                case "緊急":
                                    tmp = new emgQuest(emgDT, content.EventName);
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
            foreach (Event cnt in emgArr)
            {
                logStr += string.Format("[{0}]{1}\n", cnt.eventTime.ToString("MM/dd HH:mm"), cnt.eventName);
            }

            log.writeLog(logStr);
        }

        public async Task asyncReget()  //取得を非同期で行う
        {
            await Task.Run(() => reGet());
        }
    }

    /*
    class emgPSO2Data   //廃止予定
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
    }
    */
}
