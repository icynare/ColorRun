using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FindMissingScriptsRecursively:EditorWindow
{
    static int go_count = 0, components_count = 0, missing_count = 0;
    static List<GameObject> missGos = new List<GameObject>();
    
    [MenuItem("Window/检查脚本丢失")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
        missGos = new List<GameObject>();
    }
    Vector2 scrollPos = Vector2.zero;
    public void OnGUI()
    {
        if (GUILayout.Button("选择GameObjects"))
        {
            FindInSelected();
            scrollPos = Vector2.zero;
        }
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginScrollView( scrollPos );
        for( int i =0;i<missGos.Count;i++ )
        {
            EditorGUILayout.ObjectField( missGos[i],typeof(GameObject),true);
        }
        EditorGUILayout.EndScrollView( );
        GUILayout.FlexibleSpace();

        EditorGUILayout.EndVertical();

    }
    private static void FindInSelected()
    {
        missGos.Clear();
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }
    
    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null) 
                {
                    s = t.parent.name +"/"+s;
                    t = t.parent;
                }
                Debug.Log (s + " has an empty script attached in position: " + i, g); 
                missGos.Add( t.gameObject );
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject);
        }
    }
}