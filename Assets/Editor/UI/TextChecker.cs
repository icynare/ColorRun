/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/30 14:32:17
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TextChecker : Editor {



    [MenuItem("Editor/所有Text增加TextSpacing组件", false,CreateUICodeWindow.Priority)]
    public static void AddTextSpaceingInSelected()
    {
        GameObject[] go = Selection.gameObjects;
		foreach (GameObject g in go)
		{
            var flag = AddTextSpaceingToGo(g);
            if (flag)
            {
				var prefabAsset = PrefabUtility.GetPrefabObject(g);
				EditorUtility.SetDirty(prefabAsset);
				AssetDatabase.SaveAssets();
            }
		}
    }

    private static bool AddTextSpaceingToGo(GameObject g)
    {
        
        bool flag = false;
        Text textComp = g.GetComponent<Text>();
		if (textComp != null)
		{
            TextSpacing tsComp = g.GetComponent<TextSpacing>();
            if(tsComp == null)
            {
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
                flag = true;
                g.AddComponent<TextSpacing>();
				Debug.LogFormat("{0} Has A Text Component but without TextSpacing, Auto Added!!",s);

            }
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			var childFlag = AddTextSpaceingToGo(childT.gameObject);
            flag = flag || childFlag;
		}
        return flag;
    }

    public const string FontPath = "Assets/EditorResources/Font/mryh.ttf";

	[MenuItem("Editor/所有Text设置雅黑字体", false, CreateUICodeWindow.Priority)]
    public static void ChangeFontInSelected()
    {
        Font font =  AssetDatabase.LoadAssetAtPath<Font>(FontPath);
        Debug.Log(font);
		GameObject[] go = Selection.gameObjects;
		foreach (GameObject g in go)
		{
			var flag = ChangeFontInGo(g,font);
			if (flag)
			{
				var prefabAsset = PrefabUtility.GetPrefabObject(g);
				EditorUtility.SetDirty(prefabAsset);
				AssetDatabase.SaveAssets();
			}
		}
    }

    private static bool ChangeFontInGo(GameObject g, Font font)
	{
		bool flag = false;
		Text textComp = g.GetComponent<Text>();
		if (textComp != null)
		{
            if( textComp.font.name == "Arial")
            {
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				flag = true;
                Debug.LogFormat("{0} using font {1},change to {2}",s,textComp.font.name,font.name );
                textComp.font = font;
            }
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			var childFlag = ChangeFontInGo(childT.gameObject, font);
			flag = flag || childFlag;
		}
		return flag;
	}

	[MenuItem("Editor/所有Text增加TextSpacing并更新粗体设置", false, CreateUICodeWindow.Priority)]
	public static void AddTextBoldToGo()
	{
        AddTextSpaceingInSelected();
		GameObject[] go = Selection.gameObjects;
		foreach (GameObject g in go)
		{
			var flag = AddTextBoldToGo(g);
			if (flag)
			{
				var prefabAsset = PrefabUtility.GetPrefabObject(g);
				EditorUtility.SetDirty(prefabAsset);
				AssetDatabase.SaveAssets();
			}
		}
	}

	private static bool AddTextBoldToGo(GameObject g)
	{
		bool flag = false;
		Text textComp = g.GetComponent<Text>();

        if (textComp != null)
        {
            FontStyle fs = textComp.fontStyle;
            var isBold = false;


            if (fs == FontStyle.Bold)
            {
                textComp.fontStyle = FontStyle.Normal;
                isBold = true;
            }
            else if( fs == FontStyle.BoldAndItalic )
            {
                textComp.fontStyle = FontStyle.Italic;
				isBold = true;
            }
                
                
			TextSpacing tsComp = g.GetComponent<TextSpacing>();
			if (tsComp != null && isBold)
			{
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				flag = true;
                tsComp.setBold = true;
				Debug.LogFormat("{0} Has is bold,set TextSpacing bold enabled!!", s);

			}

		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform)
		{
			//Debug.Log("Searching " + childT.name  + " " );
			var childFlag = AddTextBoldToGo(childT.gameObject);
			flag = flag || childFlag;
		}
		return flag;
	}
}
