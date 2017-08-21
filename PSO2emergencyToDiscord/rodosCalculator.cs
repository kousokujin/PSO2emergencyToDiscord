using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO2emergencyToDiscord
{
    static class rodosCalculator
    {
        static DateTime epoch = new DateTime(2017, 5, 12);
        /*
            バル・ロドスVHのデイリーオーダーは6日毎*15回
            その3日後にも来る。
            エポックになる日は、3日後のバル・ロドスVHデイリーオーダーの日。

            エポックとなる日の例
            2017/2/8
            2017/5/12
            2017/8/13
            2017/11/14

            など
            http://monochrome9646.blog.jp/15 を参照。

            エポックより前のバル・ロドスの日は計算できない(たぶん)
        */

        static private int calcDifference(DateTime dt) //epochとの差の日数を計算
        {
            DateTime dtFix = new DateTime(dt.Year, dt.Month, dt.Day);
            TimeSpan ts = dtFix - epoch;
            return ts.Days;
        }

        static public bool calcRodosDay(DateTime dt)   //ロドスの日だったらtrueを返す。
        {
            bool a = (calcDifference(dt) % 93 == 0);
            bool b = ((calcDifference(dt) % 93) % 6 == 0);

            return a || b;
        }

        static public DateTime nextRodosDay(DateTime dt)    //次のロドスの日
        {
            TimeSpan ts1 = new TimeSpan(24, 0, 0);
            DateTime outDT = dt;

            while (!calcRodosDay(outDT))
            {
                outDT += ts1;
            }

            return outDT;
        }
    }
}
