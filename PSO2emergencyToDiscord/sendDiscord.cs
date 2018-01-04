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
        dialog dag; //ダイアログ

        public sendDiscord()
        {
            //log.writeLog(url);
            dag = new dialog();
            load();
            setDiscord();
        }

        public sendDiscord(string url)
        {
            dag = new dialog();
            this.url = url;
            setDiscord();
        }

        public void setDiscord()
        {
            wc = new WebClient();
            //wc.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            //wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            wc.Encoding = Encoding.UTF8;

            log.writeLog("Discordに接続しました。");
        }

        public async Task<string> sendContent(string text)
        {
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            string data = DynamicJson.Serialize(new
            {
                content = text
            });

            ;

            string error = await Task.Run(() =>{
                string er = "NO_ERROR";
                try
                {
                    wc.UploadString(url, data);
                }
                catch (System.ArgumentException ex)
                {
                    er = "URL_ERROR";
                }
                catch (System.Net.WebException ex)
                {
                    er = "POST_ERROR";
                    /*
                    dag.windowTitle = "投稿エラー";
                    dag.titleStr = "投稿に失敗しました。";
                    dag.detail = ex.Message;
                    dag.show();
                    log.writeLog(string.Format("投稿に失敗しました:{0}", ex.Message));
                    */
                }

                return er;
            });

            return error;

            /*
            try
            {
                await Task.Run(() => wc.UploadString(url, data));
                log.writeLog(string.Format("投稿「{0}」", text));
            }
            catch (System.ArgumentException)
            {
                dag.windowTitle = "URLエラー";
                dag.titleStr = "URLが正しくないです。";
                dag.detail = "正しいURLを入力してください。";
                dag.show();
                log.writeLog(string.Format("正しくないURLです。URL:{0}」", url));
            }
            catch (System.Net.WebException ex)
            {
                dag.windowTitle = "投稿エラー";
                dag.titleStr = "投稿に失敗しました。";
                dag.detail = ex.Message;
                dag.show();
                log.writeLog(string.Format("投稿に失敗しました:{0}", ex.Message));
            }
            */
        }

        public async Task<string> sendPicture(string filename)
        {
            //byte[] resData;
            //データを送信し、また受信する
            byte[] ts = await Task.Run(() => {
                byte [] resData = wc.UploadFile(url, filename);
                return resData;
                });
            //受信したデータを表示する
            string resText = System.Text.Encoding.UTF8.GetString(ts);

            return resText;
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
