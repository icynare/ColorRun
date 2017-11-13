/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/02 10:33:21
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
public class AssetBundleBuilder : Editor
{
	public const int PRIORITY = 80;
	//此处添加需要命名的资源后缀名,注意大小写。
	static string[] Filtersuffix = new string[] { ".prefab", ".mat", ".dds", ".png", ".ttf", ".mp3", ".avi", ".fnt", ".fontsettings",".txt" };

	//设置ab名称时默认会先清空一下
	[MenuItem("Editor/设置AssetBundle名称", false, PRIORITY)]
	public static void SetResourcesAssetBundleName()
	{
        ClearResourcesAssetBundleName();
		string fullPath = AssetBundleConfig.RESOURCE_PATH;
		StringBuilder sb = new StringBuilder(1000);
		sb.AppendLine("资源清单");
		if (Directory.Exists(fullPath))
		{
			DirectoryInfo dir = new DirectoryInfo(fullPath);

			var files = dir.GetFiles("*", SearchOption.AllDirectories);
			;
			for (var i = 0; i < files.Length; ++i)
			{
				//Debug.Log(files[i].Name);

				var fileInfo = files[i];
				EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...",
					1f * i / files.Length);
				foreach (string suffix in Filtersuffix)
				{
					if (fileInfo.Name.EndsWith(suffix))
					{

						string path = fileInfo.FullName.Replace('\\',
						'/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
						sb.AppendLine(path);

						do
						{
							var importer = AssetImporter.GetAtPath(path);
							TextureImporter texImporter = importer as TextureImporter;
							if (texImporter)
							{
								Debug.Log("Find texture");
								if (!string.IsNullOrEmpty(texImporter.spritePackingTag))
								{
									texImporter.assetBundleName = string.Format(AssetBundleConfig.ATLAS_PATH, texImporter.spritePackingTag) + AssetBundleConfig.SUFFIX;
									break;
								}
							}

							if (importer)
							{
								//这里Atlas要做特殊处理，同个atlas（Packing Tag相同）的要打包到同一个文件

								string name = path.Substring(fullPath.Substring(AssetBundleConfig.PROJECT_PATH.Length).Length);
								importer.assetBundleName = name.Substring(0, name.LastIndexOf('.')) + AssetBundleConfig.SUFFIX;
								break;
							}
							Debug.LogFormat("Impoter is null:{0}".Color(System.ConsoleColor.Red), path);
						} while (false);

					}
				}
			}
			AssetDatabase.RemoveUnusedAssetBundleNames();
		}
		EditorUtility.ClearProgressBar();
		Debug.Log(sb.ToString());
	}

	[MenuItem("Editor/输出AssetBundle名称", false, PRIORITY)]
	public static void GetAllAssetBundleName()
	{

		string[] names = AssetDatabase.GetAllAssetBundleNames();

		foreach (var name in names)
		{
			Debug.Log(name);
		}

	}

    //设置ab名称时默认会先清空一下
	//[MenuItem("Editor/清空Assetbundle设置", false, PRIORITY)]
	public static void ClearResourcesAssetBundleName()
	{

		StringBuilder sb = new StringBuilder(1000);
		string fullPath = AssetBundleConfig.RESOURCE_PATH;

		if (Directory.Exists(fullPath))
		{
			DirectoryInfo dir = new DirectoryInfo(fullPath);

			var files = dir.GetFiles("*", SearchOption.AllDirectories);
			;
			for (var i = 0; i < files.Length; ++i)
			{
				//Debug.Log(files[i].Name);

				var fileInfo = files[i];
				EditorUtility.DisplayProgressBar("清除AssetName名称", "正在清除AssetName名称中...",
					1f * i / files.Length);
				foreach (string suffix in Filtersuffix)
				{
					if (fileInfo.Name.EndsWith(suffix))
					{

						string path = fileInfo.FullName.Replace('\\',
						'/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
						sb.AppendLine(path);
						var importer = AssetImporter.GetAtPath(path);
						if (importer)
						{
							importer.assetBundleName = null;
						}
						else
						{
							Debug.LogFormat("Impoter is null:{0}".Color(System.ConsoleColor.Red), path);
						}
					}
				}
			}
			AssetDatabase.RemoveUnusedAssetBundleNames();
		}
		EditorUtility.ClearProgressBar();
	}

	[MenuItem("Editor/打包当前平台AssetBundle", false, PRIORITY)]
	public static void BuildCurAssetsBundle()
	{
#if UNITY_IOS
        BuildAssetsBundle(BuildTarget.iOS);
#elif UNITY_ANDROID
		BuildAssetsBundle(BuildTarget.Android);
#elif UNITY_STANDALONE
        BuildAssetsBundle(BuildTarget.StandaloneWindows);
#else
        Debug.Log("Just for iOS,Android and Standalone only.");
        return;
#endif

	}

	public static void BuildAssetsBundle(BuildTarget target)
	{
		string assetbundlePath = "";
		//assetbundleco
		switch (target)
		{
			case BuildTarget.iOS:
				assetbundlePath = AssetBundleConfig.GetAssetsBundlePath(AssetBundleConfig.IOS);
				break;
			case BuildTarget.Android:
				assetbundlePath = AssetBundleConfig.GetAssetsBundlePath(AssetBundleConfig.Android);
				break;
			case BuildTarget.StandaloneWindows:
				assetbundlePath = AssetBundleConfig.GetAssetsBundlePath(AssetBundleConfig.Windows);
				break;
			default:
				break;
		}
		//string assetbundlePath =  
		if (Directory.Exists(AssetBundleConfig.ASSETBUNDLE_COMMON_PATH))
		{
			Directory.Delete(AssetBundleConfig.ASSETBUNDLE_COMMON_PATH, true);
			AssetDatabase.Refresh();
		}


		Directory.CreateDirectory(assetbundlePath);
		AssetDatabase.Refresh();

		//第一个参数获取的是AssetBundle存放的相对地址。
		BuildPipeline.BuildAssetBundles(
			assetbundlePath.Substring(AssetBundleConfig.PROJECT_PATH.Length),
            BuildAssetBundleOptions.ChunkBasedCompression |
			BuildAssetBundleOptions.DeterministicAssetBundle,
			target);
		AssetDatabase.Refresh();
	}

	//     public static void ProcessBuildCMD(bool setABName,bool packAB,bool doBuild,string path)
	//     {
	//         if (setABName)
	//             SetResourcesAssetBundleName();
	//         if (packAB)
	//             BuildCurAssetsBundle();
	//         if (doBuild)
	//             SwitchPlatformEditor.BuildCurTargetPackage(path);
	// 
	//  }


}