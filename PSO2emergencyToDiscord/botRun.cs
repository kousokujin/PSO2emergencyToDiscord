using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    class botRun
    {
        sendDiscord discord;
        getPSO2 pso2;

        //次の緊急の情報
        string nextEmg;
        DateTime nextEmgTime;

        //次の通知の時間
        DateTime nextNofity;
        int nextInterval;
        bool notify;

        //次の緊急の取得の時間
        DateTime nextReload;

        //バル・ロドス通知の有効・無効
        public bool rodosNotify;
        private bool rodosDay;  //ロドスの日かどうか

        public botRun(sendDiscord discord,getPSO2 PSO2)
        {
            this.discord = discord;
            this.pso2 = PSO2;
            //notify = false;
            rodosNotify = true;
            rodosDay = rodosCalculator.calcRodosDay(DateTime.Now);

            reloadEmg();
            getEmg();
            //calcNextNofity();
            runTime();
            //printToday();
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
                            discord.sendContent(string.Format("【緊急開始】{0,2:D2}:{1:D2} {2}", nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                            getEmg();
                            calcNextNofity();
                        }
                        else
                        {
                            discord.sendContent(string.Format("【{0}分前】{1,2:D2}:{2:D2} {3}", nextInterval, nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                            calcNextNofity();
                        }

                        //notify = false;
                    }

                    if (DateTime.Compare(dt, nextReload) > 0) //緊急クエスト再取得・バル・ロドスの日の投稿
                    {
                        reloadEmg();
                        rodosDay = rodosCalculator.calcRodosDay(dt);    //ロドスの日更新

                        string emgStr = "";

                        for (int i = 0; i < pso2.emgArr.Count; i++)
                        {
                            emgStr += (string.Format("{0,2}:{1:D2} {2:D2}",
                                pso2.emgArr[i].time.Hour,
                                pso2.emgArr[i].time.Minute,
                                pso2.emgArr[i].name) + Environment.NewLine);
                        }

                        if (pso2.emgArr.Count > 0)  //緊急クエストが1つ以上ある時のみ(水曜日メンテ対策)
                        {
                            if (rodosDay == true && rodosNotify == true)  //ロドスの日
                            {
                                discord.sendContent(
                                    string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr +
                                    "なお、今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。"
                                );
                            }
                            else
                            {
                                discord.sendContent(
                                    string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr
                                );
                            }
                        }
                        else
                        {
                            if(rodosDay == true && rodosNotify == true)  //ロドスの日
                            {
                                discord.sendContent("今日はデイリーオーダー「バル・ロドス討伐(VH)」の日です。");
                            }

                        }
                    }

                    //23時30分になったらロドス警告
                    if(rodosDay && DateTime.Compare(dt,rodosRemind) > 0)
                    {
                        //次のロドスの計算
                        DateTime nextRodos =rodosCalculator.nextRodosDay(DateTime.Now + new TimeSpan(24, 0, 0));

                        if (rodosNotify == true)
                        {
                            discord.sendContent(
                                "デイリーオーダー「バル・ロドス討伐(VH)」の日があと30分で終わります。オーダーは受注しましたか？" + Environment.NewLine +
                                string.Format("次回のバル・ロドス討伐(VH)の日は{0}月{1}日です。", nextRodos.Month, nextRodos.Day)
                                );
                        }
                        rodosDay = false;   //この通知を出した時このアプリでロドスVHの日は終わる。
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

            if(DateTime.Compare(dt+ts30,nextEmgTime) > 0) //次の通知は緊急発生時
            {
                nextInterval = 0;
                nextNofity = nextEmgTime;
            }
            else
            {
                if (DateTime.Compare(dt, nextEmgTime - ts60) < 0) //次の通知は緊急の1時間前
                {
                    nextInterval = 60;
                    nextNofity = nextEmgTime - ts60;
                }
                else
                {   
                    //次の通知は緊急の30分前
                    nextInterval = 30;
                    nextNofity = nextEmgTime - ts30;
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

                /*
                if(i == pso2.emgArr.Count - 1)  //通知する緊急がなかった時
                {
                    //notify = false;
                    log.writeLog("今日の緊急はすべて終了しました。");
                }
                */
            }

            if(notify == false)
            {
                log.writeLog("通知する緊急クエストがありません。");
            }
        }

        public void reloadEmg() //再取得をする。
        {
            pso2.reGet();
            getEmg();
            calcNextNofity();

            //次の取得の時間を計算
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            TimeSpan nextDay = new TimeSpan(24, 0, 0);
            nextReload =  dt + nextDay;

            log.writeLog(string.Format("次の緊急クエストの取得は{0}月{1}日{2}時{3}分です。",nextReload.Month,nextReload.Day,nextReload.Hour,nextReload.Minute));
        }

        //デバッグ用
        private void printToday()
        {
            reloadEmg();
            string emgStr = "";
            DateTime dt = DateTime.Now;

            for (int i = 0; i < pso2.emgArr.Count; i++)
            {
                emgStr += (string.Format("{0,2}:{1:D2} {2:D2}",
                    pso2.emgArr[i].time.Hour,
                    pso2.emgArr[i].time.Minute,
                    pso2.emgArr[i].name) + Environment.NewLine);
            }

            discord.sendContent(
                string.Format("{0}月{1}日の緊急クエストは以下の通りです。", dt.Month, dt.Day) + Environment.NewLine + emgStr
            );
        }

    }
}
