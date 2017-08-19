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

        public botRun(sendDiscord discord,getPSO2 PSO2)
        {
            this.discord = discord;
            this.pso2 = PSO2;
            getEmg();
            calcNextNofity();
        }

        public async void runTime() //通知を行う(非同期)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    DateTime dt = DateTime.Now;

                }
            });
        }

        private void calcNextNofity()   //次の通知の時間を計算
        {
            DateTime dt = DateTime.Now;
            //DateTime dt = new DateTime(2017, 8, 20, 6, 40, 0);
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

            log.writeLog(string.Format("次の通知は{0}時{1}分です。", nextNofity.Hour, nextNofity.Minute));

        }

        private void getEmg()   //次の緊急クエストを更新
        {
            for(int i = 0; i < pso2.emgArr.Count(); i++)    //foreach文にしなきゃ(使命感)
            {
                if(DateTime.Compare(DateTime.Now,pso2.emgArr[i].time) < 0)
                {
                    nextEmgTime = pso2.emgArr[i].time;
                    nextEmg = pso2.emgArr[i].name;
                    log.writeLog(string.Format("次の緊急は{0}時{1}分の\"{2}\"です。", nextEmgTime.Hour, nextEmgTime.Minute, nextEmg));
                    break;
                }
            }
        }


    }
}
