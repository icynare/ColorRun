/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/08 11:54:28
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class MarcoEditor : AssetPostprocessor
{
	const int PRIORITY = 10;

#if UNITY_DEV
    [MenuItem("Editor/关闭UNITY_DEV", false, PRIORITY)]
#else
	[MenuItem("Editor/打开UNITY_DEV", false, PRIORITY)]
#endif
	public static void ToggleDev()
	{
#if UNITY_DEV
		MacroUtility.Set("UNITY_DEV", false);
#else
		MacroUtility.Set("UNITY_DEV", true);
#endif
	}
}



public class MacroUtility
{
    public static BuildTargetGroup[] allBuildTargetGroup = { BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android };

	public static void Set(string _type, bool _value)
	{
		if (EditorApplication.isPlaying)
		{
			Debug.LogError("游戏运行时无法切换环境");
			return;
		}
		if (EditorApplication.isCompiling)
		{
			Debug.LogError("游戏编译时无法切换环境");
			return;
		}
		if (Get(_type) != _value)
		{
            foreach( var targetGroup in allBuildTargetGroup )
            {
				var defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
				var defsAry = defs.Split(';');
				if (_value)
				{
					ArrayUtility.Add(ref defsAry, _type);
				}
				else
				{
					ArrayUtility.Remove(ref defsAry, _type);
				}

				defs = string.Join(";", defsAry);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defs);
            }

		}
	}

	public static bool Get(string _type)
	{
		var defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(GetCurrentTargetGroup());
		return ArrayUtility.Contains(defs.Split(';'), _type);
	}

	private static BuildTargetGroup GetCurrentTargetGroup()
	{
		switch (EditorUserBuildSettings.activeBuildTarget)
		{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneWindows64:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneLinuxUniversal:
				return BuildTargetGroup.Standalone;
			case BuildTarget.Android:
				return BuildTargetGroup.Android;
			case BuildTarget.iOS:
				return BuildTargetGroup.iOS;
			case BuildTarget.WebGL:
				return BuildTargetGroup.WebGL;
			default:
				Assert.IsTrue(false, "未处理现在已选定的平台");
				return BuildTargetGroup.Unknown;
		}
	}
}
