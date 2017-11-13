using UnityEngine;
using System.Collections;
using System.IO;

namespace UGUIFrameWorkEditor
{
	public class ChangeScriptTemplates : UnityEditor.AssetModificationProcessor
	{

		// 添加脚本注释模板
		private static string str =
		"/*********************************************************************\r\n"
		+ "* Author：ChenKaiBin \r\n"
        + "* Mail：ChenKaiBin@talkmoney.cn\r\n"
		+ "* CreateTime：#CreateTime#\r\n"
        + "* Description：\r\n\r\n"
		+ "*********************************************************************\r\n"
        + "* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved\r\n"
        + "*********************************************************************/\r\n\r\n";  
  
		// 创建资源调用
		public static void OnWillCreateAsset(string path)
		{
			// 只修改C#脚本
			path = path.Replace(".meta", "");
			if (path.EndsWith(".cs"))
			{
                string code = File.ReadAllText(path);
                if (code.StartsWith("/*********************************************************************"))
                    return;
				string allText = str;
				allText += code;
				// 替换字符串为系统时间
				allText = allText.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
				File.WriteAllText(path, allText);
			}
		}
	}
}
