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

        public botRun(sendDiscord discord,getPSO2 PSO2)
        {
            this.discord = discord;
            this.pso2 = PSO2;
            //notify = false;

            reloadEmg();
            getEmg();
            //calcNextNofity();
            runTime();
        }

        public async void runTime() //通知を行う(非同期)
        {
            await Task.Run(() =>
            {
               while (true)
               {
                    DateTime dt = DateTime.Now;
                    if ((DateTime.Compare(dt, nextNofity) > 0) && notify == true)   //次の通知の時間を現在時刻が超えた時
                    {
                        if (nextInterval == 0)
                        {
                            discord.sendContent(string.Format("【緊急開始】[{0:D2}:{1:D2}]{2}", nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                            getEmg();
                            calcNextNofity();
                        }
                        else
                        {
                            discord.sendContent(string.Format("【{0}分前】[{1:D2}:{2:D2}]{3}", nextInterval, nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                            calcNextNofity();
                        }

                        //notify = false;
                    }

                    if (DateTime.Compare(dt, nextReload) > 0) //緊急クエスト再取得
                    {
                        reloadEmg();
                        string emgStr = "";

                        for (int i = 0; i < pso2.emgArr.Count; i++)
                        {
                            emgStr += (string.Format("[{0}:{1:D2}]{2:D2}",
                                pso2.emgArr[i].time.Hour,
                                pso2.emgArr[i].time.Minute,
                                pso2.emgArr[i].name) + Environment.NewLine);
                        }

                        System.Threading.Thread.Sleep(10000);

                        discord.sendContent(
                            string.Format("{0}月{1}日の緊急クエストは以下の通りです。",dt.Month,dt.Day) + Environment.NewLine + emgStr
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

    }
}
