﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace PSO2emergencyToDiscord
{
    class botRun
    {
        sendDiscord discord;
        getPSO2 pso2;

        //画像関係
        todayEventImage teImage;
        eventImage eImage;
        simpleText st;

        //次の緊急の情報
        //Todo:次の通知のデータをevent型で管理するようにする
        //string nextEmg;
        //DateTime nextEmgTime;
        Event nextEmg;

        //次の通知の時間
        DateTime nextNofity;
        int nextInterval;
        bool notify;

        //次の緊急の取得の時間
        DateTime nextReload;

        //日付が変わった時に通知する時間
        DateTime nextDayNtf;

        //バル・ロドス通知の有効・無効
        public bool rodosNotify;
        private bool rodosDay;  //ロドスの日かどうか

        //画像でのPOSTのスイッチ
        public bool picturepost;

        public botRun(sendDiscord discord,getPSO2 PSO2)
        {
            this.discord = discord;
            this.pso2 = PSO2;
            //notify = false;
            rodosNotify = true;
            rodosDay = rodosCalculator.calcRodosDay(DateTime.Now);

            reloadEmg();
            //getEmg();
            Task t = asyncReloademg();
            //calcNextNofity();
            runTime();
            //printToday();

            nextDayNtf = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day , 0, 0, 0);
            nextDayNtf += new TimeSpan(1, 0, 0, 0);

            picturepost = false;

        }

        public async void runTime() //通知を行う(非同期)
        {
            DateTime dt;
            DateTime rodosRemind;

            await Task.Run(() =>
            {
               while (true)
               {
                    dt = DateTime.Now;
                    
                    //ロドスの日が終わるのを警告する時刻
                    rodosRemind = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 30, 0);

                    if ((DateTime.Compare(dt, nextNofity) > 0) && notify == true)   //次の通知の時間を現在時刻が超えた時
                    {
                        if (nextInterval == 0)
                        {
                            Task t = discord.sendContent(string.Format("【緊急開始】{0,2:D2}:{1:D2} {2}", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextEmg.eventName));
                            getEmg();
                            calcNextNofity();
                        }
                        else
                        {
                            string nextPOST = getLiveEmgStr((emgQuest)nextEmg);
                            Task t = discord.sendContent(string.Format("【{0}分前】{1,2:D2}:{2:D2} {3}", nextInterval, nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextPOST));
                            calcNextNofity();
                        }

                        //notify = false;
                    }

                    if (DateTime.Compare(dt, nextDayNtf) > 0) //日付が変わったら実行される
                    {
                        //reloadEmg();
                        rodosDay = rodosCalculator.calcRodosDay(dt);    //ロドスの日更新

                        string emgStr = genEmgStr();

                        /*
                        for (int i = 0; i < pso2.emgArr.Count; i++)
                        {
                            emgStr += (string.Format("{0,2}:{1:D2} {2:D2}",
                                pso2.emgArr[i].time.Hour,
                                pso2.emgArr[i].time.Minute,
                                pso2.emgArr[i].name) + Environment.NewLine);
                        }
                        */

                        /*
                         　 2017/11/18日追記
                            メモ:pso2.emgArrは水曜メンテナンス後に1週間分まとめて取得するようにしたので水曜日も空きにはならない。
                            ToDo:その日の緊急の数を取得するメソッド(戻り値int)を作り、それが0ではない場合のみ通知を行うようにする。 -> 完了
                        */
                        //if (pso2.emgArr.Count > 0)  //緊急クエストが1つ以上ある時のみ(水曜日メンテ対策)
                        if(getEmgCount() > 0)
                        {
                            if (rodosDay == true && rodosNotify == true)  //ロドスの日
                            {
                                Task t = discord.sendContent(
                                    string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr +
                                    "なお、今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。"
                                );
                            }
                            else
                            {
                                Task t = discord.sendContent(
                                    string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr
                                );
                            }
                        }
                        else
                        {
                            if(rodosDay == true && rodosNotify == true)  //ロドスの日
                            {
                                Task t = discord.sendContent("今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。");
                            }

                        }

                        //日付が変わった時の通知の日を更新
                        nextDayNtf = new DateTime(dt.Year, dt.Month, dt.Day + 1, 0, 0, 0);


                    }

                    //23時30分になったらロドス警告
                    if(rodosDay && DateTime.Compare(dt,rodosRemind) > 0)
                    {
                        //次のロドスの計算
                        DateTime nextRodos =rodosCalculator.nextRodosDay(DateTime.Now + new TimeSpan(24, 0, 0));

                        if (rodosNotify == true)
                        {
                            Task t = discord.sendContent(
                                "デイリーオーダー「バル・ロドス討伐(VH)」の日があと30分で終わります。オーダーは受注しましたか？" + Environment.NewLine +
                                string.Format("次回のバル・ロドス討伐(VH)の日は{0}月{1}日です。", nextRodos.Month, nextRodos.Day)
                                );
                        }
                        rodosDay = false;   //この通知を出した時このアプリでロドスVHの日は終わる。
                    }

                    //毎週水曜日17:00に緊急クエスト取得
                    if(DateTime.Compare(dt,nextReload) > 0)
                    {
                        reloadEmg();
                        getEmg();
                        string emgStr = genEmgStr();
                        Task t = discord.sendContent(
                                    string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr
                                );
                    }

                    System.Threading.Thread.Sleep(10000);
                }
            });
        }

        private void calcNextNofity()   //次の通知の時間を計算
        {
            DateTime dt = DateTime.Now;
            //DateTime dt = new DateTime(2017, 8, 20, 23, 0, 0);
            TimeSpan ts30 = new TimeSpan(0, 30, 0);
            TimeSpan ts60 = new TimeSpan(1, 0, 0);

            if(DateTime.Compare(dt+ts30,nextEmg.eventTime) > 0) //次の通知は緊急発生時
            {
                nextInterval = 0;
                nextNofity = nextEmg.eventTime;
            }
            else
            {
                if (DateTime.Compare(dt, nextEmg.eventTime - ts60) < 0) //次の通知は緊急の1時間前
                {
                    nextInterval = 60;
                    nextNofity = nextEmg.eventTime - ts60;
                }
                else
                {   
                    //次の通知は緊急の30分前
                    nextInterval = 30;
                    nextNofity = nextEmg.eventTime - ts30;
                }
            }

            //notify = true;
            log.writeLog(string.Format("次の通知は{0}時{1}分です。", nextNofity.Hour, nextNofity.Minute));

        }

        private void getEmg()   //次の緊急クエストを更新
        {
            DateTime dt = DateTime.Now;
            //DateTime dt = new DateTime(2017, 8, 20, 23, 0, 0);
            notify = false;

            /*
            for (int i = 0; i < pso2.emgArr.Count(); i++)    //foreach文にしなきゃ(使命感)
            {
                if(DateTime.Compare(dt,pso2.emgArr[i].time) < 0)
                {
                    nextEmgTime = pso2.emgArr[i].time;
                    nextEmg = pso2.emgArr[i].name;
                    notify = true;
                    log.writeLog(string.Format("次の緊急は{0}時{1}分の\"{2}\"です。", nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                    break;
                }
            }
        */

            foreach(Event d in pso2.emgArr)
            {
                if (DateTime.Compare(dt, d.eventTime) < 0 && d.GetType().Name == "emgQuest")
                {
                    //emgQuest emg = (emgQuest)d;
                    nextEmg = d;

                    notify = true;
                    log.writeLog(string.Format("次の緊急は{0}時{1}分の\"{2}\"です。", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextEmg.eventName));
                    break;
                }
            }

            if(notify == false)
            {
                log.writeLog("通知する緊急クエストがありません。");
            }
        }

        private string getLiveEmgStr(emgQuest e)   //クーナライブがある時に使う
        {
            if(e.liveEnable == true)
            {
                if (Regex.IsMatch(e.live, "^クーナスペシャルライブ「.*」") == true) //他のライブの時は無理
                {
                    //もっといい方法がありそう
                    string str = Regex.Replace(e.live, "^クーナスペシャルライブ「", "");
                    str = Regex.Replace(str, "」$", "");
                    return string.Format("{0}->{1}", str, e.eventName);
                }
                else
                {
                    return string.Format("ライブ・{0}", e.eventName);
                }
            }

            return e.eventName;
        }

        public void reloadEmg() //再取得をする。
        {
            pso2.reGet();
            getEmg();
            calcNextNofity();

            //次の取得の時間を計算
            //DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            //TimeSpan nextDay = new TimeSpan(24, 0, 0);int getDays = 7-((int)dt.DayOfWeek+4)%7;    //この先の緊急を取得する日数
            int getDays = 7 - ((int)DateTime.Now.DayOfWeek + 4) % 7;
            if (getDays == 7)   //水曜日の時
            {
                DateTime dt1700 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);   //今日の17:00
                if (DateTime.Compare(DateTime.Now, dt1700) <= 0)
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
            //nextReload =  dt + nextDay;
            nextReload = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day , 17, 0, 0) + new TimeSpan(getDays,0,0,0);

            log.writeLog(string.Format("次の緊急クエストの取得は{0}月{1}日{2}時{3}分です。",nextReload.Month,nextReload.Day,nextReload.Hour,nextReload.Minute));
        }

        public async Task asyncReloademg()  //非同期で実行
        {
            await Task.Run(() => reloadEmg());
        }

        private string genEmgStr()  //今日の緊急の文字列を生成
        {
            string emgStr = "";
            DateTime dt = DateTime.Now;

            DateTime toDay00 = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            DateTime toDay01 = new DateTime(dt.Year, dt.Month, dt.Day , 0, 0, 0);
            toDay01 += new TimeSpan(1, 0, 0, 0);
            foreach (Event d in pso2.emgArr)
            {
                if (DateTime.Compare(d.eventTime, toDay00) >= 0 && DateTime.Compare(d.eventTime, toDay01) < 0 && d.GetType().Name != "casino")
                {
                    emgStr += (string.Format("{0,2}:{1:D2} {2:D2}",
                        d.eventTime.Hour,
                        d.eventTime.Minute,
                        d.eventName) + Environment.NewLine);
                }
            }

            return emgStr;

        }

        private int getEmgCount()
        {
            DateTime toDay00 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime toDay01 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            toDay01 += new TimeSpan(1, 0, 0, 0);

            int count = 0;

            foreach (Event d in pso2.emgArr)
            {
                if (DateTime.Compare(d.eventTime, toDay00) >= 0 && DateTime.Compare(d.eventTime, toDay01) < 0)
                {
                    count++;
                }
            }

            
            return count;
        }

        private Event[] getEmgArr()
        {
            Event[] output = new Event[getEmgCount()];

            DateTime toDay00 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime toDay01 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            toDay01 += new TimeSpan(1, 0, 0, 0);

            int i = 0;

            foreach (Event d in pso2.emgArr)
            {
                if (DateTime.Compare(d.eventTime, toDay00) >= 0 && DateTime.Compare(d.eventTime, toDay01) < 0)
                {
                    output[i] = d;
                    i++;
                }
            }

            return output;
        }

        public void postDaily() //日付が変わったらやるPOST
        {
            DateTime dt = DateTime.Now;
            if(picturepost == true)
            {
                Event[] todayEvent = getEmgArr();
                teImage.drawString(string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day));
                
                foreach(Event env in todayEvent)
                {
                    string time = string.Format("{0,2}:{1:D2}", env.eventTime.Hour, env.eventTime.Minute);
                    teImage.drawOneline(time, env.eventName);
                }

                if (rodosDay == true && rodosNotify == true)  //ロドスの日
                {
                    teImage.drawString("今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。");
                }
            }
            else
            {
                string emgStr = genEmgStr();

                if (rodosDay == true && rodosNotify == true)  //ロドスの日
                {
                    Task t = discord.sendContent(
                        string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr +
                        "なお、今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。"
                    );
                }
                else
                {
                    Task t = discord.sendContent(
                        string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr
                    );
                }
            }
        }

    }
}
