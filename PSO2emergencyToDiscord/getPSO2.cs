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

            //緊急の情報を取得
            string data = "\""+ dt.ToString("yyyyMMdd")+"\"";

            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string jsonRaw = wc.UploadString(url, data);

            //パース
            dataParse = DynamicJson.Parse(jsonRaw);
            emgArr = new List<emgPSO2Data>();

            if (emgArr != null && emgArr.Count != 0) //emgArrの要素をすべて削除
            {
                emgArr.Clear();
            }

            foreach (var content in dataParse)
            {
                //var month = content.month;
                DateTime emgDT = new DateTime(DateTime.Now.Year, (int)content.month, (int)content.date, (int)content.hour, 0, 0);
                emgPSO2Data tmp = new emgPSO2Data(emgDT, content.evant);
                emgArr.Add(tmp);
            }


            log.writeLog("緊急クエストの情報を取得しました。");
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
