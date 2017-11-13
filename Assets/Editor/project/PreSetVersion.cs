using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Net;
using System;

public class PreSetVersion : Editor
{

    static string URL_GET = "http://119.23.155.182:7379/get/{0}";
    static string URL_SET = "http://119.23.155.182:7379/set/{0}/{1}";
    public static int IncereaseType = 0;

    [MenuItem("Editor/查看当前最新版本号", false, 200)]
    public static void PrintOnlineVersion()
    {
        CustomVersion cv = GetOnlineClientVersion();
    }

    [MenuItem("Editor/增加当前最新版本号", false, 200)]
    public static CustomVersion IncreaseOnlineVersion(CustomVersion.Platform pf = CustomVersion.Platform.None)
    {
        CustomVersion cv = GetOnlineClientVersion();
        if (pf != CustomVersion.Platform.None)
            cv.SetPlatform(pf);
        cv.Increase();
        Debug.LogFormat("Increare Online Version To:{0}", cv);
        SetOnlineClientVersion(cv);
        return cv;
    }

    [MenuItem("Editor/获取当前最新版本号", false, 200)]
    public static void PullOnlineVersion(CustomVersion cv = null)
    {
        if (cv == null)
            cv = GetOnlineClientVersion();
        SetLocalVersion(cv);
    }

    public static void SetLocalVersion(CustomVersion cv, CustomVersion.Platform pf = CustomVersion.Platform.None)
    {
        Debug.LogFormat("SetLocalVersion:{0}", cv.GetCurPlatformVersionName(false));
        PlayerSettings.bundleVersion = cv.GetCurPlatformVersionName(false);
        if( CustomVersion.Platform.None == pf )
        {
#if UNITY_IOS
            PlayerSettings.iOS.buildNumber = cv.GetCurPlatformVersionCode().ToString();
#elif UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = cv.GetCurPlatformVersionCode();
#endif
        }
        else if ( pf == CustomVersion.Platform.Android )
        {
            PlayerSettings.Android.bundleVersionCode = cv.GetCurPlatformVersionCode();
        }
        else if( pf == CustomVersion.Platform.iOS )
        {
            PlayerSettings.iOS.buildNumber = cv.GetCurPlatformVersionCode().ToString();
        }

    }

    public static CustomVersion GetOnlineClientVersion()
    {
        var url_get = string.Format(URL_GET, ProjectSettings.VERSION_KEY);
        //var url_get = string.Format(URL_GET, "test");
		WebClient wc = new WebClient();
		var text = wc.DownloadString(url_get);
        Regex regex = new Regex("{\"get\":\"(?<ver_str>.*?)\"}");
        Match match = regex.Match(text);
        string ver_str = match.Groups["ver_str"].Value;
        CustomVersion cv = new CustomVersion(ver_str);
        Debug.LogFormat("OnlineVersion:{0} CurVersionName:{1} CurVersionCode:{2}",
                        ver_str, cv.GetCurPlatformVersionName(),cv.GetCurPlatformVersionCode());
        return cv;
	}

    public static bool SetOnlineClientVersion(CustomVersion cv )
	{
        //var url_get = string.Format(URL_SET, "test", cv.GetOnlineVersionName());
        var url_get = string.Format(URL_SET, ProjectSettings.VERSION_KEY,cv.GetOnlineVersionName());
        try{
			WebClient wc = new WebClient();
			var text = wc.DownloadString(url_get);
            Debug.Log("Set Ver:"+text);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
        return false;
	}
}
    