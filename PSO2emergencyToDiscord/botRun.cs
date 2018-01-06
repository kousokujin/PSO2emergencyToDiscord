using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Drawing;

namespace PSO2emergencyToDiscord
{
    class botRun
    {
        sendDiscord discord;
        getPSO2 pso2;

        //画像関係
        public configPicture cp;
        int height = 1000;
        //Font fnt;
        //Brush fontColor;

        //次の緊急の情報
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
        //public bool picturepost;


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
            //printToday();

            nextDayNtf = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day , 0, 0, 0);
            nextDayNtf += new TimeSpan(1, 0, 0, 0);

            loadPicture();
            //fnt = new Font(cp.fontname, cp.fontsize);
            height = 1000;
            //fontColor = new SolidBrush(Color.FromArgb(255, cp.r, cp.g, cp.b));

            runTime();

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
                        postEmg();
                        if (nextInterval == 0)
                        {
                            //Task t = discord.sendContent(string.Format("【緊急開始】{0,2:D2}:{1:D2} {2}", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextEmg.eventName));
                            getEmg();
                            calcNextNofity();
                        }
                        else
                        {
                            //string nextPOST = getLiveEmgStr((emgQuest)nextEmg);
                            //Task t = discord.sendContent(string.Format("【{0}分前】{1,2:D2}:{2:D2} {3}", nextInterval, nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextPOST));
                            calcNextNofity();
                        }

                        //notify = false;
                    }

                    if (DateTime.Compare(dt, nextDayNtf) > 0) //日付が変わったら実行される
                    {
                        //reloadEmg();
                        rodosDay = rodosCalculator.calcRodosDay(dt);    //ロドスの日更新

                        //string emgStr = genEmgStr();


                        //if (pso2.emgArr.Count > 0)  //緊急クエストが1つ以上ある時のみ(水曜日メンテ対策)
                        /*
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
                        */
                        postDaily();

                        //日付が変わった時の通知の日を更新
                        nextDayNtf = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0) + new TimeSpan(1,0,0,0);


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

        private string getLiveEmgStr(emgQuest e,string section = "->")   //クーナライブがある時に使う
        {
            if(e.liveEnable == true)
            {
                if (Regex.IsMatch(e.live, "^クーナスペシャルライブ「.*」") == true) //他のライブの時は無理
                {
                    //もっといい方法がありそう
                    string str = Regex.Replace(e.live, "^クーナスペシャルライブ「", "");
                    str = Regex.Replace(str, "」$", "");
                    return string.Format("{0}{1}{2}", str,section,e.eventName);
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
            if(cp.enabled == true)
            {
                Font fnt = new Font(cp.fontname, cp.fontsize);
                Brush fontColor = new SolidBrush(Color.FromArgb(cp.a, cp.r, cp.g, cp.b));

                string fn = "deilypost.png";
                todayEventImage teImage = new todayEventImage(cp.width, height, fn, fnt, fontColor);

                if (getEmgCount() > 0)
                {
                    Event[] todayEvent = getEmgArr();
                    teImage.setParameter(cp.field1, 0, 0);
                    teImage.drawString(string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day));

                    foreach (Event env in todayEvent)
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
                    if (rodosDay == true && rodosNotify == true)  //ロドスの日
                    {
                        teImage.drawString("今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。");
                    }
                }

                teImage.Trimming();
                teImage.saveImage();
                Task t = discord.sendPicture(teImage.filename);

                //teImage.Dispose();
            }
            else
            {
                string emgStr = genEmgStr();

                if (getEmgCount() > 0)
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
                    if (rodosDay == true && rodosNotify == true)
                    {
                        Task t = discord.sendContent("今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。");
                    }
                }
            }
        }

        public void postEmg()   //60,30,緊急開始時のPOST
        {
            if (nextInterval == 0)
            {
                if (cp.enabled == true)
                {
                    string fn = "postEmg.png";
                    Font fnt = new Font(cp.fontname, cp.fontsize);
                    Brush fontColor = new SolidBrush(Color.FromArgb(cp.a, cp.r, cp.g, cp.b));

                    eventImage eImage = new eventImage(cp.width, height, fn, fnt,fontColor);
                    eImage.setParameter(cp.field2, 0, cp.field3, 0, 0);

                    string time = string.Format("{0,2}:{1:D2}", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute);
                    eImage.drawText("【緊急開始】", time, nextEmg.eventName);

                    eImage.Trimming();
                    eImage.saveImage();
                    Task t = discord.sendPicture(eImage.filename);

                    //eImage.Dispose();
                }
                else
                {
                    Task t = discord.sendContent(string.Format("【緊急開始】{0,2:D2}:{1:D2} {2}", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextEmg.eventName));
                }
                //getEmg();
                //calcNextNofity();
            }
            else
            {
                if (cp.enabled == true)
                {
                    string fn = "postEmg.png";
                    Font fnt = new Font(cp.fontname, cp.fontsize);
                    Brush fontColor = new SolidBrush(Color.FromArgb(cp.a, cp.r, cp.g, cp.b));

                    eventImage eImage = new eventImage(cp.width, height, fn, fnt, fontColor);
                    eImage.setParameter(cp.field2, 0, cp.field3, 0, 0);

                    string nextPOST = getLiveEmgStr((emgQuest)nextEmg, "\n");
                    string time = string.Format("{0,2}:{1:D2}", nextEmg.eventTime.Hour, nextEmg.eventTime.Minute);
                    eImage.drawText(string.Format("【{0}分前】",nextInterval), time, nextPOST);

                    eImage.Trimming();
                    eImage.saveImage();
                    Task t = discord.sendPicture(eImage.filename);

                    //eImage.Dispose();
                }
                else
                {
                    string nextPOST = getLiveEmgStr((emgQuest)nextEmg);
                    Task t = discord.sendContent(string.Format("【{0}分前】{1,2:D2}:{2:D2} {3}", nextInterval, nextEmg.eventTime.Hour, nextEmg.eventTime.Minute, nextPOST));
                }
                //calcNextNofity();
            }
        }

        public void postText(string str)
        {
            if(cp.enabled == true)
            {
                string fn = "text.png";
                Font fnt = new Font(cp.fontname, cp.fontsize);
                Brush fontColor = new SolidBrush(Color.FromArgb(cp.a, cp.r, cp.g, cp.b));

                simpleText st = new simpleText(cp.width, height, fn, fnt,fontColor);

                st.drawText(str);
                st.Trimming();
                st.saveImage();
                Task t = discord.sendPicture(st.filename);

                //st.Dispose();
            }
            else
            {
                Task t = discord.sendContent(str);
            }
        }

        public void savePicture()
        {
            string filename = "Picture.xml";
            xmlIO.saveObject(cp, filename);

        }

        public void loadPicture()
        {
            string filename = "Picture.xml";
            if (System.IO.File.Exists(filename) == false)
            {
                cp = new configPicture();
                cp.fontname = "MS Gothic";
                cp.fontsize = 20;

                cp.r = 255;
                cp.g = 255;
                cp.b = 255;
                cp.a = 255;

                cp.width = 600;
                cp.field1 = 80;
                cp.field2 = 150;
                cp.field3 = 80;

                cp.enabled = false;

                xmlIO.saveObject(cp, filename);
            }
            else
            {
                //もっといい方法がありそう
                configPicture copy = new configPicture();
                cp = (configPicture)xmlIO.loadObject(filename, copy.GetType());
            }
        }

    }
}
