using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class StrUtil
{
	
	public static string getMa5PassStr(string str)
	{
		byte [] b = Encoding.Default.GetBytes(str);
		MD5 md5 = new MD5CryptoServiceProvider();
		byte [] c = md5.ComputeHash(b);
		
		return System.BitConverter.ToString(c).Replace("-","");
		
	}

    public static List<Vector3> strToVector3(string moveList)
    {
        if (moveList != null && moveList.Length > 0)
        {
            List<Vector3> list = new List<Vector3>();
            string[] movePath = moveList.Split(',');

            for (int i = 0; i < movePath.Length - 2; i = i + 3)
            {
                Vector3 v = new Vector3(
                float.Parse(movePath.GetValue(i).ToString()),
                float.Parse(movePath.GetValue(i + 1).ToString()),
                float.Parse(movePath.GetValue(i + 2).ToString())
                );
                list.Add(v);
            }
            return list;
        }
        return null;
    }


    public static string FormatStringNoIndex(string format, string regex, params string[] param)
    {
        if (format == null || param == null || param.Length <= 0)
        {
            return format;
        }
        string[] formatArray = format.Split(new string[] { regex }, StringSplitOptions.None);
        string resultString = "";

        int formatArrayCount = formatArray.Length;
        int paramCount = param.Length;
        for (int i = 0; i < formatArrayCount; ++i)
        {
            resultString += formatArray[i];
            if (i < paramCount && i < formatArrayCount - 1)
            {
                resultString += param[i];
            }
            else
            {
                if (i < formatArrayCount - 1)
                {
                }
            }
        }

        return resultString;
    }

    public static Dictionary<string, string> ParseUrl(string text)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        StringReader reader = new StringReader(text);
        while (true)
        {
            string str;
            do
            {
                str = reader.ReadLine();
                if (str == null)
                {
                    return dictionary;
                }
            }
            while (((str == string.Empty) || str.StartsWith("//")) || str.StartsWith("#"));
            char[] separator = new char[] { '&' };
            foreach (string str2 in str.Split(separator))
            {
                int index = str2.IndexOf('=');
                if (index > 0)
                {
                    string str3 = str2.Substring(0, index);
                    string str4 = str2.Substring(index + 1);
                    dictionary[str3] = str4;
                }
            }
        }
    }
    private static StringBuilder m_CommonStringBuilder = new StringBuilder();
    public static StringBuilder EmptyStringBuilder
    {
        get
        {
            m_CommonStringBuilder.Remove(0, m_CommonStringBuilder.Length);
            return m_CommonStringBuilder;
        }
    }

    public static StringBuilder CommonStringBuilder
    {
        get
        {
            return m_CommonStringBuilder;
        }
    }

    public static string GetMD5String(string source)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        byte[] b = provider.ComputeHash(bytes);
        Guid guid = new Guid(b);
        return guid.ToString("N");
    }

    /// <summary>
    /// 路径分隔器转换(反斜杠转正斜杠)
    /// </summary>
    /// <param name="str">原路径</param>
    /// <returns></returns>
    public static string PathSeparatorConvert(string path)
    {
        return !string.IsNullOrEmpty(path) ? path.Replace('\\', System.IO.Path.AltDirectorySeparatorChar) : string.Empty;
    }

    /// <summary>
    /// 将秒格式化为00:00:00
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string FormatSecondToString(System.Int64 second)
    {
        System.Int64 mysecond = second % 60;
        System.Int64 myminute = ((second - mysecond) / 60) % 60;
        System.Int64 myhour = (second - mysecond) / 3600;

        if (second <= 60)
            return string.Format("00:00:{0}", mysecond);
        else if (second > 60 && second < 3600)
            return string.Format("00:{0}:{1}", myminute, mysecond);
        else
            return string.Format("{0}:{1}:{2}", myhour, myminute, mysecond);
    }

    /// <summary>
    /// 按参数索引格式化字符串
    /// </summary>
    /// <param name="format">字符串格式串，格式符号说明：{@参数索引:参数注释}</param>
    /// <param name="args">替代的参数列表, 可以用string[]，也可以用:str1,str2,str3 ....</param>
    /// <returns></returns>
    public static string FormatStringByIndex(string format, params string[] args)
    {
        if (args == null || args == null || args.Length <= 0)
        {
            return format;
        }

        MatchCollection replaceArray = Regex.Matches(format, @"\{@\d+:[^\}]*\}");

        List<string> param = new List<string>(args);
        int replaceArrayCount = replaceArray.Count;
        int paramCount = param.Count;
        for (int i = 0; i < replaceArrayCount; ++i)
        {
            string replaceStr = replaceArray[i].Value;
            MatchCollection numArray = Regex.Matches(replaceStr, @"\d+");
            if (numArray.Count >= 1)
            {
                int index;
                int.TryParse(numArray[0].Value, out index);
                if (index < paramCount)
                {
                    format = format.Replace(replaceStr, param[index]);
                }
                else
                {
                    format = format.Replace(replaceStr, "{@Error:param not enough}");
                }
            }
            else
            {
                format = format.Replace(replaceStr, "{@Error:param index error}");
            }
        }

        return format;
    }

    /// <summary>
    /// 将float数保留小数点后2位
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static string FormatFloatToString(float f)
    {
        string fstring = string.Format("{0:F}", f);
        return fstring;
    }

}
