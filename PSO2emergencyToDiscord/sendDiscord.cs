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

        public sendDiscord(string url)
        {
            this.url = url;
            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            wc.Encoding = Encoding.UTF8;
        }

        public void sendContent(string text)
        {
            var data = DynamicJson.Serialize(new
            {
                content = text
            });

            wc.UploadString(url, data);
        }

        //-------以下イベント----------
    }
}
