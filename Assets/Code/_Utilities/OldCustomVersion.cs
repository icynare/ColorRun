/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/09/27 17:47:30
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//obsolete
public class OldCustomVersion:Object
{
    public int Major = 1;
    public int Minor = 0;


    //public string IOSVersionName;
    public int IOSBuild = 0;

    //public string AndroidVersionName;
    public int AndroidBuild = 0;

    //public string WindowsVersionName;
    public int WindowsBuild = 0;

    public bool isEditor = false;
    public bool isDev = false;

    public OldCustomVersion(string versionStr = null)
    {
#if UNITY_EDITOR
		isEditor = true;
#endif
#if UNITY_DEV
        isDev = true;
#endif
		if (versionStr == null)
            InitByRuntimeVersion();
        else
            InitByCommonStr(versionStr);
    }

    public void InitByCommonStr(string versionStr = "1.0.0-0-0")
    {

        var versionParts = versionStr.Split('.');
        if (versionParts.Length == 3)
        {
            var major = int.Parse(versionParts[0]);
            var minor = int.Parse(versionParts[1]);
            var buildStr = versionParts[2];
            var buildParts = buildStr.Split('-');
            var iosBuild = 0;
            var androidBuild = 0;
            var windowsBuild = 0;
            if (buildParts.Length == 1)
            {
                androidBuild = int.Parse(buildParts[0]);
                iosBuild = androidBuild;
                windowsBuild = androidBuild;
            }
            else if (buildParts.Length == 3)
            {
                androidBuild = int.Parse(buildParts[0]);
                iosBuild = int.Parse(buildParts[1]);
                windowsBuild = int.Parse(buildParts[2]);
            }

            Major = major;
            Minor = minor;
            AndroidBuild = androidBuild;
            IOSBuild = iosBuild;
            WindowsBuild = windowsBuild;
        }
        else
        {
            Major = 1;
            Minor = 0;
            AndroidBuild = 0;
            IOSBuild = 0;
            WindowsBuild = 0;
        }
    }

    public void InitByRuntimeVersion()
    {
        var verStr = Application.version;
		var versionParts = verStr.Split('.');
		if (versionParts.Length == 3)
		{
			var major = int.Parse(versionParts[0]);
			var minor = int.Parse(versionParts[1]);
            var build = int.Parse(versionParts[2]);

			var iosBuild = build;
			var androidBuild = build;
			var windowsBuild = build;

			Major = major;
			Minor = minor;
			AndroidBuild = androidBuild;
			IOSBuild = iosBuild;
			WindowsBuild = windowsBuild;
		}
		else
		{
			Major = 1;
			Minor = 0;
			AndroidBuild = 0;
			IOSBuild = 0;
			WindowsBuild = 0;
		}

	}

	//0 = equal
	//-1 cur version is older
	//2 cur version is newer
	public bool NeedUpdate(string onlineStr)
    {
        OldCustomVersion online = new OldCustomVersion(onlineStr);
        return NeedUpdate(online);
    }
    //0 = equal
    //-1 cur version is older
    //2 cur version is newer
    public bool NeedUpdate(OldCustomVersion online)
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
		OldCustomVersion other = new OldCustomVersion(otherStr);
		return GetCurPlatformVersionCode() - other.GetCurPlatformVersionCode();
	}
	//0 = equal
	//-1 cur version is older
	//2 cur version is newer
	public int CompareBuild(OldCustomVersion other)
	{
		return GetCurPlatformVersionCode() - other.GetCurPlatformVersionCode();
	}

    public string GetCurPlatformVersionName()
    {
#if UNITY_IOS
		return IOSVersionName;
#elif UNITY_ANDROID
        return AndroidVersionName;
#elif UNITY_STANDALONE
        return WindowsVersionName;
#endif
	}

    public int GetCurPlatformVersionCode()
    {
#if UNITY_IOS
        return IOSVersionCode;
#elif UNITY_ANDROID
        return AndroidVersionCode;
#elif UNITY_STANDALONE
        return WindowsVersionCode;
#endif
	}

    //t = 0 build
    //t = 1 minor
    //t = 2 major
    public void Increase(int t = 0)
    {
        if( t == 2 )
        {
            Major += 1;
        }
        else if( t == 1 )
        {
            Minor += 1;
        }
        else
        {
#if UNITY_IOS
			IOSBuild+=1;
#elif UNITY_ANDROID
            AndroidBuild += 1;
#elif UNITY_STANDALONE
            WindowsBuild += 1;
#endif
		}


	}

    public string IOSVersionName
    {
        get{
            if( isDev )
	            return string.Format("{0}.{1}.{2}.dev",Major,Minor,IOSBuild);
            else
                return string.Format("{0}.{1}.{2}", Major, Minor, IOSBuild);
        }
    }

	public string AndroidVersionName
	{
		get
		{
			if (isDev)
				return string.Format("{0}.{1}.{2}.dev", Major, Minor, AndroidBuild);
			else
				return string.Format("{0}.{1}.{2}", Major, Minor, AndroidBuild);
		}
	}

	public string WindowsVersionName
	{
		get
		{
            return string.Format("{0}.{1}.{2}", Major, Minor, WindowsBuild);
			if (isDev)
				return string.Format("{0}.{1}.{2}.dev", Major, Minor, AndroidBuild);
			else
				return string.Format("{0}.{1}.{2}", Major, Minor, AndroidBuild);
		}
	}

	public int IOSVersionCode
	{
		get
		{
            return Major*100000+Minor*1000+IOSBuild;
		}
	}

	public int AndroidVersionCode
	{
		get
		{
            return Major * 100000 + Minor * 1000 + AndroidBuild;
		}
	}

	public int WindowsVersionCode
	{
		get
		{
            return Major * 100000 + Minor * 1000 + WindowsBuild;
		}
	}

    public override string ToString()
    {
        return string.Format("[VersionName:{0} VersionCode:{1}",
            GetCurPlatformVersionName(), GetCurPlatformVersionCode());
    }

}