/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/09/27 17:47:30
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class CustomVersion:UnityEngine.Object
{
    public enum Platform{
        iOS,Android,Windows,None
    }


    private Platform _platform = Platform.None;

    //public string IOSVersionName;
    private int IOSCode = 0;

    //public string AndroidVersionName;
    private int AndroidCode = 0;

    //public string WindowsVersionName;
    private int WindowsCode = 0;

    public bool isEditor = false;
    public bool isDev = false;

    private string oldVersionStr = "";
    private int metaVersion = 0;

    public CustomVersion(string versionStr = null)
    {
#if UNITY_EDITOR
		isEditor = true;
#endif
#if UNITY_DEV
        isDev = true;
#endif
#if UNITY_IOS
        SetPlatform(Platform.iOS);
#elif UNITY_ANDROID
        SetPlatform(Platform.Android);
#elif UNITY_STANDALONE
        SetPlatform(Platform.Windows);
#endif
        try{
            if (versionStr == null)
                InitByRuntimeVersion();
            else
                InitByCommonStr(versionStr);
        }
        catch( Exception e)
        {
            Debug.Log( e);
        }
    }

    public void SetPlatform(Platform platform)
    {
        _platform = platform;
    }

    public void InitByCommonStr(string versionStr = "1.0.0-0-0")
    {
        Debug.LogFormat("VersionStr:{0}",versionStr);
        var versionParts = versionStr.Split('.');
        if (versionParts.Length == 3)
        {
            var major = int.Parse(versionParts[0]);
            var minor = int.Parse(versionParts[1]);
            var buildStr = versionParts[2];
            var buildParts = buildStr.Split('-');
            var iosBuild = major*100000+minor*1000;
            var androidBuild = major * 100000 + minor * 1000;
            var windowsBuild = major * 100000 + minor * 1000;

            oldVersionStr = string.Format("{0}.{1}.{2}", major, minor, 0);


            if( buildParts.Length <=3 && buildParts.Length>=0 ) 
            {
                //metaVersion = -1 历史遗留
                if( buildParts.Length == 3 )
                {
                    androidBuild = int.Parse(buildParts[0]);
                    iosBuild = int.Parse(buildParts[1]);
                    windowsBuild = int.Parse(buildParts[2]);
                }
                else{//1 or 2
                    androidBuild = int.Parse(buildParts[0]);
                    iosBuild = androidBuild;
                    windowsBuild = androidBuild; 
                }
                AndroidCode = androidBuild;
                IOSCode = iosBuild;
                WindowsCode = windowsBuild;
                Debug.Log("Old Version Format!");
                return;
            }

        }
        int metaVer = CheckVersion(versionStr);
        metaVersion = metaVer;
        switch (metaVer)
        {
            case 1:
                //1.0.0-1@1_1_2-1_1_5-1_1_3
                ParseVersion1(versionStr);
                Debug.Log("Version Format 1");
                return;
                break;
            default:
                Debug.LogFormat("Not Parser For MetaVer:{0} VerStr:{1}", metaVer, versionStr);
                break;
        }
        Debug.Log("Parse Fail VerStr:{1}");
        AndroidCode = 100000;
        IOSCode = 100000;
        WindowsCode = 100000;
    }

    private int CheckVersion(string verStr)
    {
        var ms = Regex.Matches(verStr, @"\d+(?=@\d)");
        if(ms.Count >0)
        {
            int metaVer = 0;
            int.TryParse(ms[0].Value, out metaVer);
            return metaVer;
        }
        return 0;
    }

    void ParseVersion1(string verStr)
    {
        verStr = verStr.Split('@')[1];
        var ms = Regex.Matches(verStr, @"\d+_\d+_\d+");
        if (ms.Count == 3)
        {
            AndroidCode = GetVersionNumByStr(ms[0].Value);
            IOSCode = GetVersionNumByStr(ms[1].Value);
            WindowsCode = GetVersionNumByStr(ms[2].Value);
        }
        else
        {
            AndroidCode = 100000;
            IOSCode = 100000;
            WindowsCode = 100000;
        }

    }

    //1_2_3-> 102003
    int GetVersionNumByStr(string verStr)
    {
        var  parts = verStr.Split('_');
        if( parts.Length==3 )
        {
            var major = int.Parse(parts[0]);
            var minor = int.Parse(parts[1]);
            var build = int.Parse(parts[2]);

            return major * 100000 + minor * 1000 + build;
        }
        return 100000;
    }

    public void InitByRuntimeVersion()
    {
        var verStr = Application.version;
        Debug.LogFormat("Application.version:{0}", verStr);
		var versionParts = verStr.Split('.');
		if (versionParts.Length == 3)
		{
			var major = int.Parse(versionParts[0]);
			var minor = int.Parse(versionParts[1]);
            var build = int.Parse(versionParts[2]);

            var iosBuild = major*100000+minor*1000+build;
            var androidBuild = iosBuild;
            var windowsBuild = iosBuild;

			AndroidCode = androidBuild;
			IOSCode = iosBuild;
			WindowsCode = windowsBuild;
		}
		else
		{

            AndroidCode = 100000;
            IOSCode = 100000;
            WindowsCode = 100000;
		}

	}

	//0 = equal
	//-1 cur version is older
	//2 cur version is newer
	public bool NeedUpdate(string onlineStr)
    {
        CustomVersion online = new CustomVersion(onlineStr);
        return NeedUpdate(online);
    }
    //0 = equal
    //-1 cur version is older
    //2 cur version is newer
    public bool NeedUpdate(CustomVersion online)
    {
        Debug.LogFormat("Local:{0} Online:{1}",this,online);
        if (isEditor)
            return false;
        else
		    return online.GetCurPlatformVersionCode() / 1000 > GetCurPlatformVersionCode() / 1000;
    }

	//0 = equal
	//-1 cur version is older
	//2 cur version is newer
	public int CompareBuild(string otherStr)
	{
		CustomVersion other = new CustomVersion(otherStr);
		return GetCurPlatformVersionCode() - other.GetCurPlatformVersionCode();
	}
	//0 = equal
	//-1 cur version is older
	//2 cur version is newer
	public int CompareBuild(CustomVersion other)
	{
		return GetCurPlatformVersionCode() - other.GetCurPlatformVersionCode();
	}

    public string GetCurPlatformVersionName( bool addDev = true )
    {
        var ver = IOSVersionName;
        if(_platform == Platform.iOS)
            ver = IOSVersionName;
        else if(_platform == Platform.Android)
            ver = AndroidVersionName;
        else if(_platform == Platform.Windows)
            ver = WindowsVersionName;

        if (isDev && addDev)
            return ver + ".dev";
        else
            return ver;
	}

    public int GetCurPlatformVersionCode()
    {
        if (_platform == Platform.iOS)
            return IOSVersionCode;
        else if (_platform == Platform.Android)
            return AndroidVersionCode;
        else if (_platform == Platform.Windows)
            return WindowsVersionCode;
        return WindowsVersionCode;


	}

    //t = 0 build
    //t = 1 minor
    //t = 2 major
    public void Increase(int t = 0)
    {
        var incr = 1;
        if (t == 2)
            incr = 100000;
        else if (t == 1)
            incr = 1000;
        if (_platform == Platform.iOS)
            IOSCode += incr;
        else if (_platform == Platform.Android)
            AndroidCode += incr;
        else if (_platform == Platform.Windows)
            WindowsCode += incr;
	}

    public string IOSVersionName
    {
        get{
            var major = IOSCode / 100000;
            var minor = (IOSCode - 100000 * major) / 1000;
            var build = IOSCode % 1000;
            return string.Format("{0}.{1}.{2}", major, minor, build);
        }
    }
    public string IOSOnlineVersionName
    {
        get
        {
            var major = IOSCode / 100000;
            var minor = (IOSCode - 100000 * major) / 1000;
            var build = IOSCode % 1000;
            return string.Format("{0}_{1}_{2}", major, minor, build);
        }
    }

	public string AndroidVersionName
	{
		get
		{
            var major = AndroidCode / 100000;
            var minor = (AndroidCode - 100000 * major) / 1000;
            var build = AndroidCode % 1000;
            return string.Format("{0}.{1}.{2}", major, minor, build);
            
		}
	}

    public string AndroidOnlineVersionName
    {
        get
        {
            var major = AndroidCode / 100000;
            var minor = (AndroidCode - 100000 * major) / 1000;
            var build = AndroidCode % 1000;
            return string.Format("{0}_{1}_{2}", major, minor, build);

        }
    }

	public string WindowsVersionName
	{
		get
		{
            var major = WindowsCode / 100000;
            var minor = (WindowsCode - 100000 * major) / 1000;
            var build = WindowsCode % 1000;
            return string.Format("{0}.{1}.{2}", major, minor, build);
		}
	}

    public string WindowsOnlineVersionName
    {
        get
        {
            var major = WindowsCode / 100000;
            var minor = (WindowsCode - 100000 * major) / 1000;
            var build = WindowsCode % 1000;
            return string.Format("{0}_{1}_{2}", major, minor, build);
        }
    }

	public int IOSVersionCode
	{
		get
		{
            return IOSCode;
		}
	}

	public int AndroidVersionCode
	{
		get
		{
            return AndroidCode;
		}
	}

	public int WindowsVersionCode
	{
		get
		{
            return WindowsCode;
		}
	}

    public override string ToString()
    {
        return string.Format("[VersionName:{0} VersionCode:{1}",
            GetCurPlatformVersionName(), GetCurPlatformVersionCode());
    }

    public string GetOnlineVersionName()
    {
        var VersionStr = string.Format("{0}@{1}-{2}-{3}",
        metaVersion,AndroidOnlineVersionName,IOSOnlineVersionName,WindowsOnlineVersionName);
        if( !string.IsNullOrEmpty(oldVersionStr) )
        {
            VersionStr = oldVersionStr + "-" + VersionStr;
        }
        return VersionStr;
    }

}