/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/09 14:17:56
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TestMultiSprite : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //Object[] objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/EditorResources/UI/Test/emoji@2x.png");
        //Debug.Log(objs);
        //foreach( var obj in objs )
        //{
        //    Debug.Log(obj);
        //}
        //AssetBundle ab =  AssetBundle.LoadFromFile(AssetBundleConfig.ASSETBUNDLE_PLATFORM_PATH + "ui/test/emoji@2x.unity3d");
        //var b = (ab.GetAllAssetNames());
        //Debug.Log(b.Length);
        //foreach (var bb in b)
        //    Debug.Log(bb);
        //var a = ab.LoadAsset<Sprite>("emoji@2x_1");
        ////var ss = ab.LoadAllAssets<Sprite>();
        ////foreach (var sss in ss)
        //    //Debug.Log(sss);
        //Debug.Log(a);
        Test2();
        Test3();


	}

    void Test2()
    {
		var a = AssetBundle.LoadFromFile(AssetBundleConfig.ASSETBUNDLE_PLATFORM_PATH + "ui/common/c3.unity3d");
        var b = a.LoadAllAssets(typeof(Object));
		Debug.Log(b);
		foreach (var bb in b ) 
		    Debug.Log(bb);
	}

    void Test3()
    {
        var a = ResourceManager.Instance.LoadSprite("Common/C3");
        Debug.Log(a);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
