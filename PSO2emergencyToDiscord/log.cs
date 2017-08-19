using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    public static class log
    {
        static StreamWriter writer;
        private static string filename;
        private static DateTime dt;
        private static string date;
        private static string time;

        public static void writeLog(string str)
        {
            dt = DateTime.Now;
            filename = "log.txt";
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            writer = new StreamWriter(filename, true, sjisEnc);

            date = dt.ToString("yyyy/MM/dd");
            time = dt.ToString(" HH:mm:ss");
            string text = string.Format("[{0}{1}]{2}",date,time,str);
            writer.WriteLine(text);

            writer.Close();
        }
    }
}
