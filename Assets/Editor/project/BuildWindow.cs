/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/14 17:25:16
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildWindow : EditorWindow
{

    static string KeystoreName = "Android_keystore/密码/borderless.keystore";
    static string KeystorePass = "borderless123";
    static string KeyaliasName = "borderless";
    static string KeyaliasPass = "borderless123";

    BuildWindow window;
    bool setABName = true;
    bool packAB = true;
    bool doBuild = true;
    bool isDev = true;
    string path = "";

    [MenuItem("Editor/打包工具", false, AssetBundleBuilder.PRIORITY - 1)]
    static void OpenWindow()
    {
        BuildWindow window = (BuildWindow)EditorWindow.GetWindowWithRect(typeof(BuildWindow), new Rect(0, 0, 400, 200));

        window.InitWindow();


    }

    private void InitWindow()
    {
        if (string.IsNullOrEmpty(path))
        {
            path = SwitchPlatformEditor.GetDefaultSavePathForCurBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log(path);
        }
    }

    public void OnGUI()
    {
        GUILayout.Space(14);
        GUILayout.Label("当前平台:" + AssetBundleConfig.ASSETBUNDLE_FOLDER_NAME);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        //GUILayout.BeginVertical();
        packAB = GUILayout.Toggle(packAB, "打包AssetBundle");
        isDev = GUILayout.Toggle(isDev, "是否开发版本");
        increaseVersion = GUILayout.Toggle(increaseVersion, "自增版本号");
        doBuild = GUILayout.Toggle(doBuild, "构建平台包");
        GUILayout.BeginHorizontal();
        GUILayout.Label("路径：", GUILayout.Width(50));


        path = GUILayout.TextField(path);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            path = SwitchPlatformEditor.AskSavePathForCurBuildTarget(EditorUserBuildSettings.activeBuildTarget);
        }
        GUILayout.EndHorizontal();
        //GUI.EndGroup();
        EditorGUILayout.Space();
        if (GUILayout.Button("开始执行"))
        {
            CommonPack(packAB, doBuild, EditorUserBuildSettings.activeBuildTarget, path, isDev,increaseVersion);
        }
    }


    public static void CommonPack(bool packab, bool dobuild, BuildTarget target, string path, bool isDev,bool increaseVersion = true)
    {

		if (isDev)
			MacroUtility.Set("UNITY_DEV", true);
		else
			MacroUtility.Set("UNITY_DEV", false);



        if (packab)
        {
            AssetBundleBuilder.SetResourcesAssetBundleName();
            AssetBundleBuilder.BuildAssetsBundle(target);
        }
        if (dobuild)
        {
            CommonBuild(target, path, isDev,increaseVersion);
        }
    }

    public static void CommonBuild(BuildTarget target, string path, bool isDev,bool increaseVersion)
    {
        PreBuild(target,increaseVersion);

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
        BuildOptions opts = BuildOptions.None;
        if (isDev)
            opts = BuildOptions.Development | BuildOptions.AllowDebugging  | BuildOptions.ConnectWithProfiler;
        else
            opts = BuildOptions.None;
        BuildPipeline.BuildPlayer(levels.ToArray(),path,target, opts);

    }

    public static void PreBuild( BuildTarget target,bool increaseVersion )
    {
        if(increaseVersion)
        {
            try
            {
                CustomVersion.Platform pf = CustomVersion.Platform.None;
                if (target == BuildTarget.iOS)
                    pf = CustomVersion.Platform.iOS;
                else if (target == BuildTarget.StandaloneWindows)
                    pf = CustomVersion.Platform.Windows;
                else if (target == BuildTarget.Android)
                    pf = CustomVersion.Platform.Android;
                    
                CustomVersion cv = PreSetVersion.IncreaseOnlineVersion(pf );
                PreSetVersion.SetLocalVersion(cv, pf);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                if (isFormCMD)
                    EditorApplication.Exit(-1);
            }
        }

        switch(target)
        {
            case BuildTarget.Android:
                PreBuildForAndroid();
                break;
            case BuildTarget.iOS:
				PreBuildForIOS();
				break;
            case BuildTarget.StandaloneWindows:
				PreBuildForWindows();
				break;

        }
    }

    public static void PreBuildForAndroid()
    {
		PlayerSettings.Android.keystoreName = KeystoreName;
		PlayerSettings.Android.keystorePass = KeystorePass;
		PlayerSettings.Android.keyaliasName = KeyaliasName;
        PlayerSettings.Android.keyaliasPass = KeyaliasPass;
    }

	public static void PreBuildForIOS()
	{

	}
	public static void PreBuildForWindows()
	{

	}

    public static void TESTCMD()
    {
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>.");
        foreach( var s in System.Environment.GetCommandLineArgs() )
        {
            Debug.Log(s);
        }
    }
    static bool isFormCMD = false;
    private bool increaseVersion = false;

    [MenuItem("Editor/测试打包", false, AssetBundleBuilder.PRIORITY - 1)]
    public static void BuildFromCMD()
    {
        isFormCMD = true;
		var pcPath = GetParamter("pcpath", "niuniu.exe");
		var androidpath = GetParamter("androidpath" , "niuniu.apk");
        var pojPath = GetParamter("pojPath","/Users/yang/Dev/yancai/niuniu");
        var isDev = bool.Parse( GetParamter("isDev","true") );

		var android = bool.Parse(GetParamter("android", "false"));
		var windows = bool.Parse(GetParamter("windows", "false"));
		var ios = bool.Parse(GetParamter("ios", "false"));

        if(android)
        {
			var androidPath = Path.Combine(pojPath, androidpath);
			CommonPack(true, true, BuildTarget.Android, androidPath, isDev);
            Debug.Log(">>>>>>>>>>BUILD ANDROID SUCCESS!!!!");
        }

		if (windows)
		{
			var winPath = Path.Combine(pojPath, pcPath + ".exe");
			CommonPack(true, true, BuildTarget.StandaloneWindows, winPath, isDev);
			Debug.Log(">>>>>>>>>>BUILD WINDOWS SUCCESS!!!!");
		}

        if(ios)
        {
			var xcodePath = Path.Combine(pojPath, "XCodePoj");
			CommonPack(true, true, BuildTarget.iOS, xcodePath, isDev);
			Debug.Log(">>>>>>>>>>BUILD IOS SUCCESS!!!!");
        }


		

    }

    private static string GetParamter(string argname, string defaultValue)
    {
        var args = System.Environment.GetCommandLineArgs().ToList();
        var argIndex = args.LastIndexOf( "-"+argname.ToLower() );
        if( argIndex >=0 && argIndex < args.Count-1 )
        {
            return args[argIndex + 1];
        }
        return defaultValue;
    }





}
