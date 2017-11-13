/*********************************************************************
* Autor：xiaozhang 
* Mail：mobile@talkmoney.cn
* CreateTime：2017/09/04 09:55:15
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindNoRefTextures
{
	private const string MenuItemText = "Editor/查找未被引用的图片";

	[MenuItem(MenuItemText, false, 100)]
	public static void Find()
	{
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();

		var referenceCache = new Dictionary<string, List<string>>();
        HashSet<string> texPaths = new HashSet<string>();
        string[] texPostfixs = { ".png", ".jpg" };

		string[] guids = AssetDatabase.FindAssets("");
		foreach (string guid in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!assetPath.StartsWith("Assets/EditorResources/"))
                continue;
            if (CheckPostfix(assetPath, texPostfixs))
            {
                texPaths.Add(assetPath);
            }
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
        Debug.Log("Build index takes " + sw.ElapsedMilliseconds + " milliseconds");

        foreach (var tp in texPaths)
        {
            if(!referenceCache.ContainsKey(tp))
            {
                //Debug.Log("No refs",);
                var obj = AssetDatabase.LoadMainAssetAtPath(tp);
                Debug.LogFormat(obj,"Not refs:{0}",tp);
            }
        }

		referenceCache.Clear();
	}

    private static bool CheckPostfix(string path, string[] postfixs)
    {
        path = path.ToLower();
        foreach (var pf in postfixs )
        {
            if (path.ToLower().EndsWith(pf))
                return true;
        }
        return false;

    }

}