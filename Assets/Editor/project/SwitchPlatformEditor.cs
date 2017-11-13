/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/08 12:36:15
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SwitchPlatformEditor : AssetPostprocessor
{
	const int PRIORITY = 30;
#if !UNITY_IOS
	[MenuItem("Editor/切换到IOS平台", false, PRIORITY)]
	public static void SwitchToIOS()
	{
		SwitchPlatform(BuildTarget.iOS);
	}
#else
    [MenuItem("Editor/>打包iOS包(todo)", false, PRIORITY)]
    public static void BuildIOS()
    {
        BuildPackage(BuildTarget.iOS);
    }
#endif

#if !UNITY_ANDROID
    [MenuItem("Editor/切换到Android平台", false, PRIORITY)]
    public static void SwitchToAndroid()
    {
        SwitchPlatform(BuildTarget.Android);
    }

#else
	[MenuItem("Editor/>打包Android包(todo)", false, PRIORITY)]
	public static void BuildAndroid()
	{
		BuildPackage(BuildTarget.Android);

	}
#endif

#if !UNITY_STANDALONE
	[MenuItem("Editor/切换到Windows平台", false, PRIORITY)]
	public static void SwitchToWindows()
	{
		SwitchPlatform(BuildTarget.StandaloneWindows);
	}
#else
    [MenuItem("Editor/>打包Windows包(todo)", false, PRIORITY)]
    public static void BuildWindows()
    {
        BuildPackage(BuildTarget.StandaloneWindows);
    }
#endif

	public static void SwitchPlatform(BuildTarget newTarget)
	{
		BuildTarget curTarget = EditorUserBuildSettings.activeBuildTarget;
		//Debug.Log("test here");
		//Debug.Log(AssetDatabase.IsValidFolder("Assets/Editor"));

		//string a = AssetDatabase.MoveAsset("Assets/StreamingAssets/AssetBundles/Android2","Assets/StreamingAssets2/AssetBundles/Android2");
		//Debug.Log(a);
		//

		//exp:step for switch from windows to android
		//move windows from real to temp
		//move android from temp to real,if not ,ask for building ab
		string folderNameToHide = GetFolderName(curTarget);
		string folderPathToHideDest = AssetBundleConfig.ASSETBUNDLE_TEMP_FROM_ASSETS + "/" + folderNameToHide;
		string folderPathToHideOri = string.Format(AssetBundleConfig.ASSETBUNDLE_FROM_ASSETS, folderNameToHide);

		if (!AssetDatabase.IsValidFolder(string.Format(AssetBundleConfig.ASSETBUNDLE_TEMP_FROM_ASSETS, "")))
		{
			string a = AssetDatabase.CreateFolder("Assets", AssetBundleConfig.ASSETBUNDLE_TEMP);
			Debug.Log(a);
		}
		if (AssetDatabase.IsValidFolder(folderPathToHideOri))
		{
			AssetDatabase.MoveAsset(folderPathToHideOri, folderPathToHideDest);
			Debug.LogFormat("Move from {0} to {1}", folderPathToHideOri, folderPathToHideDest);
		}

		string folderNameToShow = GetFolderName(newTarget);
		string folderPathToShowDest = string.Format(AssetBundleConfig.ASSETBUNDLE_FROM_ASSETS, folderNameToShow);
		string folderPathToShowOri = AssetBundleConfig.ASSETBUNDLE_TEMP_FROM_ASSETS + "/" + folderNameToShow;
		bool notAssetBundle = false;

		if (AssetDatabase.IsValidFolder(folderPathToShowOri))
		{
			AssetDatabase.MoveAsset(folderPathToShowOri, folderPathToShowDest);
			Debug.LogFormat("Move from {0} to {1}", folderPathToShowOri, folderPathToShowDest);
		}
		else
		{
			notAssetBundle = true;
		}


		EditorUserBuildSettings.SwitchActiveBuildTarget(newTarget);

		if (notAssetBundle)
		{
			bool a = EditorUtility.DisplayDialog("提示", "未打包该平台AssetBundle，是否打包？", "确定", "取消");
			if (a)
			{
				AssetBundleBuilder.BuildAssetsBundle(newTarget);
			}

		}
	}

	public static string GetFolderName(BuildTarget target)
	{
		string name;
		switch (target)
		{
			case BuildTarget.Android:
				name = AssetBundleConfig.Android;
				break;
			case BuildTarget.iOS:
				name = AssetBundleConfig.IOS;
				break;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				name = AssetBundleConfig.Windows;
				break;
			default:
				name = "other";
				break;
		}
		return name;
	}

	public static string AskSavePathForCurBuildTarget(BuildTarget target)
	{
		string[] strs = Application.dataPath.Split(("/").ToCharArray());
		Debug.Log(Application.dataPath);
		string pojName = strs[strs.Length - 2];
		string path = null;

		if (target == BuildTarget.Android)
			path = EditorUtility.SaveFilePanel("保存位置", "", pojName, "apk");
		else if (target == BuildTarget.iOS)
			path = EditorUtility.SaveFolderPanel("保存位置", "", pojName);
		else if (target == BuildTarget.StandaloneWindows)
			path = EditorUtility.SaveFolderPanel("保存位置", "", pojName);
		return path;
	}

	public static string GetDefaultSavePathForCurBuildTarget(BuildTarget target)
	{
		string[] strs = Application.dataPath.Split(("/").ToCharArray());
		Debug.Log("------");
		string pojName = strs[strs.Length - 2];
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

        if (target == BuildTarget.Android)
            path += pojName + ".apk";
        else if (target == BuildTarget.iOS)
            path += pojName+"XCode";
		else if (target == BuildTarget.StandaloneWindows)
			path += pojName;
		return path;
	}

	public static void BuildPackage(BuildTarget target)
	{

		string path = AskSavePathForCurBuildTarget(target);
		BuildPackage(target, path);
	}

	public static void BuildCurTargetPackage(string path)
	{
#if UNITY_IOS
        BuildPackage(BuildTarget.iOS,path);
#elif UNITY_ANDROID
		BuildPackage(BuildTarget.Android, path);
#elif UNITY_STANDALONE
        BuildPackage(BuildTarget.StandaloneWindows,path);
#else
        Debug.Log("Just for iOS,Android and Standalone only.");
        return;
#endif
	}

	public static void BuildPackage(BuildTarget target, string path)
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		List<string> levels = new List<string>();
		for (int i = 0; i < scenes.Length; i++)
		{
			if (scenes[i].enabled)
			{
				levels.Add("" + scenes[i].path);
				Debug.Log("" + scenes[i].path);
			}
		}
#if UNITY_DEV
        BuildPipeline.BuildPlayer(levels.ToArray(), path, target, BuildOptions.AllowDebugging | BuildOptions.Development);
#else
		BuildPipeline.BuildPlayer(levels.ToArray(), path, target, BuildOptions.None);
#endif



	}

}
