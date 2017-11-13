/*********************************************************************
* Autor：xiaozhang 
* Mail：mobile@talkmoney.cn
* CreateTime：2017/08/26 10:49:48
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.XCodeEditor;
using System.Collections;
using UnityEditor.Callbacks;

public class PBXProjectSetting
{
	//该属性是在build完成后，被调用的callback
	[PostProcessBuildAttribute(0)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
	{
		// BuildTarget需为iOS
		if (buildTarget != BuildTarget.iOS)
			return;
		// 初始化
		var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject pbxProject = new PBXProject();
		pbxProject.ReadFromFile(projectPath);
		string targetGuid = pbxProject.TargetGuidByName("Unity-iPhone");

		// 添加flag
		pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
		// 关闭Bitcode
		pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

		// 添加framwrok
		pbxProject.AddFrameworkToProject(targetGuid, "Security.framework", false);
		pbxProject.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
		pbxProject.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
		pbxProject.AddFrameworkToProject(targetGuid, "CoreAudio.framework", false);

		//添加lib
		AddLibToProject(pbxProject, targetGuid, "libsqlite3.tbd");
		AddLibToProject(pbxProject, targetGuid, "libstdc++.6.0.9.tbd");
		AddLibToProject(pbxProject, targetGuid, "libz.tbd");

		

		// 修改Info.plist文件
		var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
		var plist = new PlistDocument();
		plist.ReadFromFile(plistPath);

		// 插入URL Scheme到Info.plsit（理清结构）
		var array = plist.root.CreateArray("CFBundleURLTypes");

		//插入dict
		var urlDict = array.AddDict();
		urlDict.SetString("CFBundleTypeRole", "Editor");
		//插入array
		var urlInnerArray = urlDict.CreateArray("CFBundleURLSchemes");
		urlInnerArray.AddString("wx9e6114e9204e6f39");

		var LSApplicationQueriesSchemes = plist.root.CreateArray("LSApplicationQueriesSchemes");
		LSApplicationQueriesSchemes.AddString("wechat");
		LSApplicationQueriesSchemes.AddString("weixin");
		LSApplicationQueriesSchemes.AddString("wx9e6114e9204e6f39");

		// 应用修改
		plist.WriteToFile(plistPath);

		//插入代码
		//读取UnityAppController.mm文件
		string unityAppControllerPath = pathToBuiltProject + "/Classes/UnityAppController.mm";
		XClass UnityAppController = new XClass(unityAppControllerPath);

		//在指定代码后面增加一行代码
		UnityAppController.WriteBelow("#import <OpenGLES/ES2/glext.h>", "#import \"WXApi.h\"");
        UnityAppController.WriteBelow("AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);", 
                                      "return  [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];");

		string unityAppControllerHeaderPath = pathToBuiltProject + "/Classes/UnityAppController.h";
		XClass unityAppControllerHeader = new XClass(unityAppControllerHeaderPath);
        unityAppControllerHeader.WriteBelow("#import <QuartzCore/CADisplayLink.h>", "#import \"PluginWx.h\"");
        unityAppControllerHeader.Replace("NSObject<UIApplicationDelegate>", "PluginWx");

        //在指定代码后面增加一大行代码
        //UnityAppController.WriteBelow("// if you wont use keyboard you may comment it out at save some memory", newCode);

        //这里添加一些附加的plist，例如游龙的plist
        CopyPLists(Application.dataPath + "/Plugins/iOS/PLists", pathToBuiltProject,pbxProject);

		// 应用修改
		File.WriteAllText(projectPath, pbxProject.WriteToString());

	}


	//添加lib方法
	static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
	{
		string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
		inst.AddFileToBuild(targetGuid, fileGuid);
	}


    internal static void CopyPLists(string srcPath, string dstPath,PBXProject pbxPoj)
    {
		foreach (var filePath in Directory.GetFiles(srcPath))
        {
            if( filePath.EndsWith(".plist") )
            {
				Debug.Log(filePath);
				var plist = new PlistDocument();
				plist.ReadFromFile(filePath);
				Debug.Log(Path.Combine(dstPath, Path.GetFileName(filePath)));
                var newPath = Path.Combine(dstPath, Path.GetFileName(filePath));
				plist.WriteToFile(newPath);
                string targetGuid = pbxPoj.TargetGuidByName("Unity-iPhone");
				string fileGuid = pbxPoj.AddFile(newPath, Path.GetFileName(newPath));
                pbxPoj.AddFileToBuild(targetGuid,fileGuid);
                //pbxPoj.

            }

        }
        
    }


	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
        {
			Directory.Delete(dstPath);
			Directory.CreateDirectory(dstPath);
		}
		if (File.Exists(dstPath))
			File.Delete(dstPath);


		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}
}
