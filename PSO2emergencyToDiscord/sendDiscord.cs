using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    class sendDiscord
    {
        string url; //WebHooksのURL
        WebClient wc;

        public sendDiscord()
        {
            //log.writeLog(url);
            load();
            setDiscord();
        }

        public sendDiscord(string url)
        {
            this.url = url;
            setDiscord();
        }

        public void setDiscord()
        {
            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            wc.Encoding = Encoding.UTF8;

            log.writeLog("Discordに接続しました。");
        }

        public void sendContent(string text)
        {
            var data = DynamicJson.Serialize(new
            {
                content = text
            });

            wc.UploadString(url, data);
            log.writeLog(string.Format("投稿「{0}」", text));
        }

        public void setUrl(string url)
        {
            this.url = url;
        }

        public string getUrl()
        {
            return this.url;
        }

        //discordのurl保存
        public void save()
        {
            //URL保存のファイル名
            string fileName = @"discordconf.xml";

            confDiscord obj = new confDiscord();
            obj.setUrl(this.url);

            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(confDiscord));

            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                fileName, false, new System.Text.UTF8Encoding(false));

            serializer.Serialize(sw, obj);

            sw.Close();
        }

        //discordのurlを読み込み
        public void load()
        {
            string fileName = @"discordconf.xml";

            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(confDiscord));

            System.IO.StreamReader sr = new System.IO.StreamReader(
                fileName, new System.Text.UTF8Encoding(false));

            confDiscord obj = (confDiscord)serializer.Deserialize(sr);

            sr.Close();

            this.url = obj.url;

            log.writeLog("設定ファイルを読み込みました。");
        }
    }

    public class confDiscord
    {
        public string url;

        public void setUrl(string url)
        {
            this.url = url;
        }
    }
}
