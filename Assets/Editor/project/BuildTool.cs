using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BuildTool:Editor
{
	[MenuItem("Assets/游戏打包/PC")]
	public static void BuildPC()
	{

		EditorBuildSettingsScene [] scenes =  EditorBuildSettings.scenes;
        List<string> levels = new List<string>();
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled)
            {
                levels.Add("" + scenes[i].path);
                Debug.Log("" + scenes[i].path);
            }
        }
        Debug.Log(">>>>:"+ System.Environment.GetCommandLineArgs()[6] );
        BuildPipeline.BuildPlayer(levels.ToArray(), System.Environment.GetCommandLineArgs()[6], BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem("Assets/游戏打包/Android")]
	public static void BuildAndroid()
	{		
		EditorBuildSettingsScene [] scenes =  EditorBuildSettings.scenes;
        List<string> levels = new List<string>();
        for( int i = 0;i< scenes.Length;i++  )
        {
            if (scenes[i].enabled)
            {
                levels.Add("" + scenes[i].path);
                Debug.Log("" + scenes[i].path);
            }
        }
        Debug.Log(">>>>:"+ System.Environment.GetCommandLineArgs()[6] );
        BuildPipeline.BuildPlayer(levels.ToArray(), System.Environment.GetCommandLineArgs()[6], BuildTarget.Android, BuildOptions.None);
	}
}