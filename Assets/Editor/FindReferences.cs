/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/09/04 11:02:20
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindReferencesInProject
{
	private const string MenuItemText = "Assets/查找所有引用";

	[MenuItem(MenuItemText, false, 25)]
	public static void Find()
	{
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();

		var referenceCache = new Dictionary<string, List<string>>();

		string[] guids = AssetDatabase.FindAssets("");
		foreach (string guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

			foreach (var dependency in dependencies)
			{
				if (referenceCache.ContainsKey(dependency))
				{
					if (!referenceCache[dependency].Contains(assetPath))
					{
						referenceCache[dependency].Add(assetPath);
					}
				}
				else
				{
					referenceCache[dependency] = new List<string>() { assetPath };
				}
			}
		}

		Debug.Log("创建索引" + sw.ElapsedMilliseconds + " ms");

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		Debug.Log("查找: " + path, Selection.activeObject);
		if (referenceCache.ContainsKey(path))
		{
			foreach (var reference in referenceCache[path])
			{
				Debug.Log(reference, AssetDatabase.LoadMainAssetAtPath(reference));
			}
		}
		else
		{
			Debug.LogWarning("找不到索引");
		}

		referenceCache.Clear();
	}

	[MenuItem(MenuItemText, true)]
	public static bool Validate()
	{
		if (Selection.activeObject)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			return !AssetDatabase.IsValidFolder(path);
		}

		return false;
	}
}