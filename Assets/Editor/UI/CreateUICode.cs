using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;


public class CreateUICodeWindow:EditorWindow
{
    public const int Priority = 110;
    // add type here 1
    public enum TypeEnum
    {
        GameObject,
        Text,
        Button,
        Dropdown,
        Image,
        InputField,
        ToggleGroup,
        Toggle,
        Scrollbar,
        ScrollRect,
        Mask

    }
    //add type here 2
    public static Type [] typeList = {typeof(GameObject), typeof(Text), typeof(Button),typeof(Dropdown),typeof(Image),typeof(InputField),typeof(ToggleGroup),typeof(Toggle),typeof(Scrollbar),typeof(ScrollRect),typeof(Mask)} ;

    public static Transform RootGO;
    public static Dictionary<GameObject,int> typeDic;
    public static Dictionary<GameObject,string> nameDic;

    public static StringBuilder codeBuilder = new StringBuilder ();

    [MenuItem("Editor/UI代码生成", false,Priority)]
    static void CallWindow()
    {
        CreateUICodeWindow window = EditorWindow.GetWindowWithRect (typeof(CreateUICodeWindow), new Rect (0, 0, 450, 582), false, "UI代码生成") as CreateUICodeWindow;
        window.Show ();
    }

    void Start()
    {

        RootGO = null;
        typeDic = new Dictionary<GameObject, int> ();
        nameDic = new Dictionary<GameObject, string> ();
    }


    void OnGUI()
    {
        if (typeDic == null) {
            Start ();
        }
        try{
            RootGO = EditorGUILayout.ObjectField ("拖入UI根节点：", RootGO, typeof(Transform), true) as Transform;

            EditorGUILayout.Separator ();

            GameObject[] selects =  Selection.gameObjects;


            GUILayout.Label ( "选中要导出的Gameobject：" );
            EditorGUILayout.BeginHorizontal ("box",( GUILayout.Width ( 450f )));//--

            GUILayout.Label ("GameObject", EditorStyles.label, GUILayout.Width (250f));
            GUILayout.FlexibleSpace ();
            GUILayout.Label ("Name(inClass)", EditorStyles.label, GUILayout.Width (100f));
            GUILayout.FlexibleSpace ();
            GUILayout.Label ("Type", EditorStyles.label, GUILayout.Width (100f));

            EditorGUILayout.EndHorizontal (); //---


            EditorGUILayout.BeginVertical ("box" , GUILayout.Width ( 400f )); //||

            foreach (GameObject go in selects) {

                GUILayout.BeginHorizontal ();

                string path = GetGameObjectPath (go.transform);
                int curType;
                if (!typeDic.TryGetValue (go, out curType)) {
                    typeDic.Add ( go,GetTypeIndexOfGo (go) );
                    curType = typeDic [go];
                }
                string name;
                if (!nameDic.TryGetValue (go, out name)) {
                    nameDic.Add ( go,go.name );
                    name = go.name;
                }
                EditorGUILayout.LabelField ( path,GUILayout.Width (250f) );
                GUILayout.FlexibleSpace ();
                name = EditorGUILayout.TextField ( name,GUILayout.Width (100f) );
                GUILayout.FlexibleSpace ();
                curType = (int)(TypeEnum)EditorGUILayout.EnumPopup ((TypeEnum)curType, GUILayout.Width (100)) ;

                typeDic [go] = curType;
                nameDic [go] = name;

                GUILayout.EndHorizontal ();

            }

            EditorGUILayout.EndVertical ();



            if (GUILayout.Button ("生成代码")) {
                ExportCode ();
            }
            //if( GUILayout.Button ( "导出类代码" ) )


            EditorGUILayout.BeginVertical ("box");
            EditorGUILayout.TextArea ( codeBuilder.ToString ());
            GUILayout.FlexibleSpace ();
            EditorGUILayout.EndVertical ();
        }
        catch( Exception e) {
            Debug.LogError ( e.Message+"\n"+e.StackTrace );
        }





    }

    string GetGameObjectPath(Transform go)
    {
        string path = go.name;
        while (go.parent != null && go.parent != RootGO) {
            go = go.parent;
            path = go.name + "/" + path;
        }
        return path;
    }

    int GetTypeIndexOfGo( GameObject go )
    {
        for (int i = 1; i < typeList.Length; i++) {
            if (go.GetComponent (typeList[i]) != null)

                return i;
        }
        return 0;
  
    }

    void ExportCode()
    {
        codeBuilder = new StringBuilder();
        GameObject[] selects =  Selection.gameObjects;

        for (int i = 0; i < selects.Length; i++) {
            GameObject go = selects [i];
            string t = "private " + (TypeEnum)typeDic[go];
            string name = nameDic [go];
            codeBuilder.Append ( t+" "+name+";\r\n" );
        }

        codeBuilder.Append ("\r\n");

        for (int i = 0; i < selects.Length; i++) {
            GameObject go = selects [i];
            string t = "" + (TypeEnum)typeDic [go];
            string name = nameDic [go];
            string path = GetGameObjectPath (go.transform);
            if (t == "GameObject") {
                codeBuilder.Append ( name + " = GetChild(\""+ path+"\");\r\n" );
            } else {
                codeBuilder.Append ( name + " = GetChild<"+t+">(\""+ path+"\");\r\n" );
            }
        }



    }

}
