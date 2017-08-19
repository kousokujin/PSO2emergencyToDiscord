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

        public getPSO2()
        {
            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.Encoding = Encoding.UTF8;

            //緊急の情報を取得
            reGet();

        }

        //再取得
        public void reGet()
        {
            //現在時刻を取得
            DateTime dt = DateTime.Now;

            //緊急の情報を取得
            string data = "\""+ dt.ToString("yyyyMMdd")+"\"";
            string jsonRaw = wc.UploadString(url, data);

            //パース
            dataParse = DynamicJson.Parse(jsonRaw);

            //デバッグ用
            /*
            foreach(var post in dataParse)
            {
                System.Console.WriteLine(post.evant);
            }
            System.Console.WriteLine();

            */

            log.writeLog("緊急クエストの情報を取得しました。");
        }
    }
}
