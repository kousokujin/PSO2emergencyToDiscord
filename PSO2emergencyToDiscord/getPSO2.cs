using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    class getPSO2
    {
        const string url = "http://akakitune87.net/api/v1/pso2ema";
        WebClient wc;
        public dynamic dataParse;
        public List<emgPSO2Data> emgArr;

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
            bool live = false;  //ライブフラグ

            //緊急の情報を取得
            string data = "\""+ dt.ToString("yyyyMMdd")+"\"";
            //string data = "\"" + "20170701" + "\"";

            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string jsonRaw = wc.UploadString(url, data);

            //パース
            dataParse = DynamicJson.Parse(jsonRaw);
            emgArr = new List<emgPSO2Data>();

            if (emgArr != null && emgArr.Count != 0) //emgArrの要素をすべて削除
            {
                emgArr.Clear();
            }

            //if (dataParse != null)
            //{
                foreach (dynamic content in dataParse)
                {
                //var month = content.month;
                /*
                    アキくんへ
                    発生時だけでなく、発生分も返すとクーナライブに対応できそうです。
                    クーナライブ→死
                 */
                    if (content.evant == "ライブ")
                    {
                        live = true;
                    }
                    else
                    {
                        if (live == true)
                        {
                            DateTime emgDT = new DateTime(DateTime.Now.Year, (int)content.month, (int)content.date, (int)content.hour, 30, 0);
                            emgPSO2Data tmp = new emgPSO2Data(emgDT, "ライブ・"+content.evant);
                            emgArr.Add(tmp);
                        }
                        else
                        {
                            DateTime emgDT = new DateTime(DateTime.Now.Year, (int)content.month, (int)content.date, (int)content.hour, 0, 0);
                            emgPSO2Data tmp = new emgPSO2Data(emgDT, content.evant);
                            emgArr.Add(tmp);

                        }
                        live = false;
                    }
                }
            //}

            string logStr = "緊急クエストの情報を取得しました。取得した緊急クエストは以下の通りです。\n";
            foreach(emgPSO2Data cnt in emgArr)
            {
                logStr += string.Format("[{0}]{1}\n", cnt.time.ToString("HH:mm"), cnt.name);
            }

            log.writeLog(logStr);
        }
    }

    class emgPSO2Data
    {
        public DateTime time;
        public string name;

        public emgPSO2Data(DateTime t,string str)
        {
            this.time = t;
            this.name = str;
        }
    }
}
