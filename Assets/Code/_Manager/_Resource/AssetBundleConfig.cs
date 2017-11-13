/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/02 10:40:50
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

//http://blog.csdn.net/y1196645376/article/details/52602002

using UnityEngine;

public class AssetBundleConfig
{
    public static string ASSETBUNDLE_TEMP = "StreamingAssetsTemp";
    //StreamingAsset缓存文件夹
    public static string ASSETBUNDLE_TEMP_FROM_ASSETS = "Assets/"+ASSETBUNDLE_TEMP;


	//Assets地址
	public static string APPLICATION_PATH = Application.dataPath + "/";

	//资源地址
	public static string RESOURCE_PATH = Application.dataPath + "/EditorResources/";

    //atlas目录
    public static string ATLAS_PATH = "ui/atlas/{0}";


	//工程地址，Assets上一层地址
	public static string PROJECT_PATH = APPLICATION_PATH.Substring(0, APPLICATION_PATH.Length - 7);

	//AssetBundle存放的文件夹名
	public static string ASSETBUNDLE_FOLDER_NAME
	{
		get
        {
#if UNITY_IOS
            return IOS;
#elif UNITY_ANDROID
            return Android;
#elif UNITY_STANDALONE
            return Windows;
#else
            Debug.Log("Just for iOS,Android and Standalone only.");
            return "";
#endif
        }
	}

	//AssetBundle打包的后缀名
	public static string SUFFIX = ".unity3d";



	public static string IOS = "IOS";
	public static string Android = "Android";
	public static string Windows = "Windows";

    public static string GetAssetsBundlePath( string platformTarget )
    {
        return  Application.dataPath + "/StreamingAssets/AssetBundles/" + platformTarget+"/";
    }

	public static string ASSETBUNDLE_FROM_ASSETS = "Assets/StreamingAssets/AssetBundles/{0}";


	//AssetBundle打包路径
	public static string ASSETBUNDLE_PATH
	{
		get
		{
			return Application.dataPath + "/StreamingAssets/AssetBundles/" + ASSETBUNDLE_FOLDER_NAME + "/";
		}
	}

	public static string ASSETBUNDLE_COMMON_PATH
	{
		get
		{
			return Application.dataPath + "/StreamingAssets/AssetBundles/";
		}
	}

	//不用直接使用Application.streamingAssetsPath,这样的出来的安卓版路径只能用www加载
	public static string ASSETBUNDLE_PLATFORM_PATH
    {
        get{
#if UNITY_EDITOR
            return Application.dataPath + "/StreamingAssets/AssetBundles/" + ASSETBUNDLE_FOLDER_NAME + "/";
#elif UNITY_IOS
            return  Application.dataPath + "/Raw/AssetBundles/"+IOS+"/";
#elif UNITY_ANDROID
            return Application.dataPath+"!assets/AssetBundles/"+Android+"/";
#elif UNITY_STANDALONE
			return Application.dataPath + "/StreamingAssets/AssetBundles/"+Windows+"/";
#else
            Debug.Log("Just for iOS,Android and Standalone only.");
            return "";
#endif
		}
    }

}
