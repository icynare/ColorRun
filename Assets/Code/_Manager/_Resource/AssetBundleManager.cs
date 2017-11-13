﻿/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/03 16:40:50
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

//ref:http://blog.csdn.net/y1196645376/article/details/52602002

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AssetBundleManager:Singleton<AssetBundleManager> {

	private AssetBundleManifest manifest = null;

	private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    private StringBuilder sbBuffer;
    private int stackDepth = 0;
    private string[] tabs = { "", "\t", "\t\t", "\t\t\t","\t\t\t\t","\t\t\t\t\t","\t\t\t\t\t\t","\t\t\t\t\t\t\t" };

    //abpath exp：UI/common/common.png
	public  AssetBundle LoadAB(string abPath)
	{
        if( stackDepth ==0 )
        {
            sbBuffer = new StringBuilder(3000);

        }
        long startTime = System.DateTime.Now.Ticks;
		if (abDic.ContainsKey(abPath) == true)
			return abDic[abPath];
		if (manifest == null)
		{
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(AssetBundleConfig.ASSETBUNDLE_PLATFORM_PATH +
			AssetBundleConfig.ASSETBUNDLE_FOLDER_NAME);
			manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
		}
		if (manifest != null)
		{
            
			// 2.获取依赖文件列表  
			string[] cubedepends = manifest.GetAllDependencies(abPath);

			for (int index = 0; index < cubedepends.Length; index++)
			{
                //Debug.Log(cubedepends[index]);
                // 3.加载所有的依赖资源
                stackDepth += 1;
				LoadAB(cubedepends[index]);
				stackDepth -= 1;

			}

            // 4.加载资源
            long realStart = System.DateTime.Now.Ticks;
            abDic[abPath] = AssetBundle.LoadFromFile(AssetBundleConfig.ASSETBUNDLE_PLATFORM_PATH + abPath);
            //long logStart = System.DateTime.Now.Ticks;
			//Debug.LogFormat("real abpath:{0}, get ab:{1}", AssetBundleConfig.ASSETBUNDLE_PLATFORM_PATH + abPath,abDic[abPath]);
			float usedTime = (float)(System.DateTime.Now.Ticks - startTime) / 10000;
			float realTime = (float)(System.DateTime.Now.Ticks - realStart)/10000;
			//Debug.Log cost a lot
			//Debug.LogFormat("Load AssetBundle {2} Total Used(include deps num:{0}) : {1}ms,RealTime {3}ms ".Color(System.ConsoleColor.Black),cubedepends.Length,usedTime,abPath,realTime);
            sbBuffer.AppendFormat("{0}Load AssetBundle {1} Total Used(include deps num:{2}) : {3}ms,RealTime {4}ms\n".Color(System.ConsoleColor.Black),
                                  tabs[stackDepth],
                                  abPath,
                                  cubedepends.Length,
                                  usedTime, 
                                  realTime);
            //float logTime = (float)(System.DateTime.Now.Ticks - logStart) / 10000;
            //Debug.Log(logTime);
            if (stackDepth == 0)
            {
				Debug.Log(sbBuffer);
                sbBuffer = null;
            }
               
            return abDic[abPath];
		}
        else
        {
			Debug.LogError("Can't find AssetBundleManifest");
			return null;
        }

	}
    
	//exp to get:prefab/login/login.prefab
	//assetName:login
	//abPath:prefab/login/login.unity3d
    private T LoadAssetFromAB<T>(string assetName,string abPath) where T:Object
	{
		//abPath = string.Format(abPath);
		if (!string.IsNullOrEmpty(abPath))
			abPath = abPath.ToLower();
		LoadAB(abPath);
        AssetBundle ab;
        if (abDic.TryGetValue(abPath,out ab))
		{
            //Debug.LogFormat("assetName:{0} ab:{1}",assetName,ab);
            return ab.LoadAsset<T>(assetName);
		}
		else
		{
            Debug.LogFormat("Load From AssetBundle Fail:abPath:{0}".Color(System.ConsoleColor.Red), abPath);
            return null;
		}
	}


	
	public Object[] LoadAllAsset( string assetPathWithoutExt,string abPath = null)
    {
        
        if(string.IsNullOrEmpty(abPath))
            abPath = assetPathWithoutExt + AssetBundleConfig.SUFFIX;
        string assetNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(assetPathWithoutExt);

		if (!string.IsNullOrEmpty(abPath))
			abPath = abPath.ToLower();
		LoadAB(abPath);
		AssetBundle ab;
		if (abDic.TryGetValue(abPath, out ab))
		{
            //Debug.LogFormat("assetName:{0} ab:{1}",assetName,ab);
            return ab.LoadAllAssets();
		}
		else
		{
			Debug.LogFormat("Load From AssetBundle Fail:abPath:{0}".Color(System.ConsoleColor.Red), abPath);
			return null;
		}
    }

	//exp to get:prefab/login/login.prefab
	//assetPathNoExt:prefab/login/login
	//abPath:prefab/login/login.unity3d
    public T LoadAsset<T>(string assetPathWithoutExt, string abPath = null ) where T:Object
    {
		if (string.IsNullOrEmpty(abPath))
			abPath = assetPathWithoutExt + AssetBundleConfig.SUFFIX;
		string assetNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(assetPathWithoutExt);
        Object obj = LoadAssetFromAB<T>(assetNameWithoutExt, abPath);
		if (obj is T)
		{
			return obj as T;
		}
		else
		{
			Debug.LogFormat("Load Assset from  Ab fail:assetPathWithoutExt:{0} abPath:{1} get Object:{2}".Color(System.ConsoleColor.Red), assetPathWithoutExt, abPath, obj);
			return null;
		}
    }


	//exp to get:ui/login/login.png
	//assetPathWithoutExt:ui/login/login
	//abPath:prefab/login/login.unity3d
	//packingTag：是否包含在atlas中，是的话，传入packingTag
	public Sprite LoadSprite( string assetPathWithoutExt,string packingTag = null,string slicedName = null )
    {
        string abPath = null;
        if(!string.IsNullOrEmpty(packingTag))
        {
            abPath = string.Format(AssetBundleConfig.ATLAS_PATH, packingTag) + AssetBundleConfig.SUFFIX;
        }
        Sprite sprite = LoadAsset<Sprite>(assetPathWithoutExt, abPath);
        return sprite;
    }

	//exp to get:ui/gameui/emoji.png->emoji_1
	//assetPathWithoutExt:ui/gameui/emoji
	//slicedName：emoji_1
	public Sprite LoadSpriteInMultiple(string assetPathWithoutExt, string slicedName)
	{
		string abPath = null;
        Object[] sprites = LoadAllAsset(assetPathWithoutExt, abPath);
        for (int i = 0; i < sprites.Length;i++)
        {
            if (sprites[i].name == slicedName)
                return sprites[i] as Sprite;
        }
        Debug.LogFormat("Load Sprite from  Ab fail:assetPathWithoutExt:{0} abPath:{1},slicedName:{2}".Color(System.ConsoleColor.Red), assetPathWithoutExt, abPath, slicedName);

		return null;

    }


}
