using UnityEngine;
using System.Collections;
using System;
public class ServerTimeManager
{
    //1970年，也就是unix为0时的datetime
	public static DateTime UNIX_START = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
	public static long serverLoginTime = 0; //ms
	public static long clientLoginTime = 0; //ms
	public static int now()//当前秒
	{
		return (int)(nowMS() / 1000);
	}

    public static long nowMS()//当前毫秒
    {
        return serverLoginTime - clientLoginTime + (long)(DateTime.Now.Ticks / 10000);
    }


    public static int getTime()//从登录到当前经过的时间
    {
        return (int)((DateTime.Now.Ticks / 10000) - clientLoginTime);
    }


	//当前服务器unix时间戳 ，ms
	public static long getServerUnixTimeMS()
	{
		return serverLoginTime - clientLoginTime + getClientUnixTimeMS();
	}
	//当前客户端unix时间戳 ，ms
	public static long getClientUnixTimeMS()
	{
		return (long)((DateTime.Now - UNIX_START).TotalMilliseconds);
	}

	//登录的时候服务器应该发一个服务端时间过来，设置一次即可使用
	public static void SetServerCurTime(long serverUnixMS)
	{
        clientLoginTime = getClientUnixTimeMS();
        serverLoginTime = serverUnixMS;
	}
	
	public static DateTime GetDateTimeByUnixTimeMS(long unixTimeMS)
    {
        return UNIX_START.AddMilliseconds(unixTimeMS);
    }

    //转换时间戳为当前时间
    public static DateTime ToRealTime(long timeUnixMS)
    {
        long ITime = timeUnixMS * 10000;
        TimeSpan toNow = new TimeSpan(ITime);
        return UNIX_START.Add(toNow);
    }
}