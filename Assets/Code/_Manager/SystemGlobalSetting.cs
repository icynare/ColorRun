using UnityEngine;
using System.Collections;

//全局通用配置
public class SystemGlobalSetting : Singleton<SystemGlobalSetting> {

    public static readonly string SCENE_MANAGER = "GameManager";

	public static int LocalProxyVersion = -1;
    public static int ServerProxyVersion = -1;

    public static void InitLocalProxyVersion()
    {
        if(LocalProxyVersion == -1 )
        {
            TextAsset ta = ResourceManager.Instance.LoadText("version");
            Debug.Log(ta);
            LocalProxyVersion = int.Parse(ta.text);
        }
    }

    public static void SetServerProxyVersion(string proxyVersion)
    {
        ServerProxyVersion = int.Parse(proxyVersion);
    }

    public static bool IsProxyVersionUpToDate()
    {
        InitLocalProxyVersion();
        Debug.LogFormat("Local:{0} Online:{1}",LocalProxyVersion,ServerProxyVersion);
        return LocalProxyVersion >= ServerProxyVersion;
    }

	public static CustomVersion LocalClientVersion = new CustomVersion();
    public static CustomVersion OnlineClientVersion = null;

	public static void SetLastestClientVersion(string ver_str)
	{
        OnlineClientVersion = new CustomVersion(ver_str);
	}

    public static bool NeedUpdateClient()
    {
        return LocalClientVersion.NeedUpdate(OnlineClientVersion);
    }


}
