using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;
//put xml into unity_editor
public class Tools
{
    //得到当前时间的时间戳，从1970年开始算
    private static long date1970Ticks = 0;
    private static float suffixLen = 0;
    private static float maxNameLen = 0;
    private const string suffix = "…";

    public static int nowTime()
    {
        if (date1970Ticks == 0)
        {
            DateTime date = new DateTime(1970, 1, 1);
            date1970Ticks = date.Ticks;
        }

        return (int)((DateTime.Now.Ticks - date1970Ticks) / 10000000);
    }

    public static long nowTimeMs()
    {
        if (date1970Ticks == 0)
        {
            DateTime date = new DateTime(1970, 1, 1);
            date1970Ticks = date.Ticks;
        }

        return (long)((DateTime.Now.Ticks - date1970Ticks) / 10000);

    }

    //时间戳转为C#格式 参数秒
    public static DateTime getDateTime(int timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
    }


    public static DateTime TimeStampToDateTime(double unixTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static double DateTimeToTimestamp(DateTime dateTime)
    {
        return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
    }

    public static double NowSeconds()
    {
        return (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
    }

    public static String GetTimeFormat(int time)  //12:0:0
    {
        StringBuilder timeStr = new StringBuilder();
        int second = time % 60;
        int minute = time / 60;
        if (minute >= 60)
        {
            time = minute / 60;
            timeStr = timeStr.Append(time > 9 ? time.ToString() : "0" + time);
            timeStr = timeStr.Append(":");
            minute = minute % 60;
            timeStr = timeStr.Append(minute > 9 ? minute.ToString() : "0" + minute);
            timeStr = timeStr.Append(":");
            timeStr = timeStr.Append(second > 9 ? second.ToString() : "0" + second);
        }
        else
        {
            timeStr = timeStr.Append("00:");
            timeStr = timeStr.Append(minute > 9 ? minute.ToString() : "0" + minute);
            timeStr = timeStr.Append(":");
            timeStr = timeStr.Append(second > 9 ? second.ToString() : "0" + second);
        }
        return timeStr.ToString();
    }

    /// <summary>
    /// 将秒格式化为00时00分00秒
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string FormatSecondToZhString(int second)
    {
        int mysecond = second % 60;
        int myminute = ((second - mysecond) / 60) % 60;
        int myhour = (second - mysecond) / 3600;

        if (second <= 60)
            return string.Format("{0}秒", mysecond);
        else if (second > 60 && second < 3600)
            return string.Format("{0}分{1}秒", myminute, mysecond);
        else
            return string.Format("{0}时{1}分{2}秒", myhour, myminute, mysecond);
    }

    private static int id = 100001;
    public static int GetID()
    {
        return id++;
    }

   static float GetStringWidth(string text, Font font, int fontSize, FontStyle fontStyle)
    {
        font.RequestCharactersInTexture(text, fontSize, fontStyle);
        CharacterInfo characterInfo;
        float width = 0f;
        for (int i = 0; i < text.Length; i++)
        {
            font.GetCharacterInfo(text[i], out characterInfo, fontSize);
            width += characterInfo.advance;
        }
        return width;
    }

    public static void StripNameWithSuffix(string nameStr,  Text nameText, string str)
    {
        //if (str.Length > 4)
        //{
        //    this.name.text = this.name.text.Substring(0, 2) + "……";
        //}
        suffixLen = GetStringWidth(suffix, nameText.font, nameText.fontSize, nameText.fontStyle);
        maxNameLen = GetStringWidth(str, nameText.font, nameText.fontSize, nameText.fontStyle);
        float nameLen = GetStringWidth(nameStr, nameText.font, nameText.fontSize, nameText.fontStyle);
        if (nameLen > maxNameLen)
        {
            int totalLength = 0;
            CharacterInfo characterInfo = new CharacterInfo();

            char[] arr = nameStr.ToCharArray();

            int i = 0;
            foreach (char c in arr)
            {
                nameText.font.GetCharacterInfo(c, out characterInfo, nameText.fontSize);

                int newLength = totalLength + characterInfo.advance;
                if (newLength > maxNameLen - suffixLen)
                {
                    Debug.LogFormat("newLength {0},  totalLength {1}: ", newLength, totalLength);
                    if (Mathf.Abs(newLength - (maxNameLen - suffixLen)) > Mathf.Abs((maxNameLen - suffixLen) - totalLength))
                    {
                        break;
                    }
                    else
                    {
                        totalLength = newLength;
                        i++;
                        break;
                    }
                }
                totalLength += characterInfo.advance;
                i++;
            }
            Debug.LogFormat("totalLength {0} : ", totalLength);
            nameStr = nameStr.Substring(0, i) + suffix;
        }

        nameText.text = nameStr;
    }

}