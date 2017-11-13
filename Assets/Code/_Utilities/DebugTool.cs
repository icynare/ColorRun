/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/08/04 18:20:35
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugTool : SingletonBehaviour<DebugTool> {

    public struct GuiStruct
    {
        public Delegate del;
        public int code;
        public GuiStruct(Delegate d, int c)
        {
            del = d;
            code = c;
        }
    }

    private Dictionary<string, GuiStruct> mylog = new Dictionary<string, GuiStruct>();

    public bool openLog = false;
    GuiStruct gs;
    public const int MAXINPUT = 100;
    private GUIStyle style;
    private Color color = Color.yellow;

    string[] str = new string[MAXINPUT];
    int guiCount = 1;
    int codeMid = 0;
    float screenHeight;
    float screenWidth;
    Vector2 scollPosition = Vector2.zero;

    private void Start()
    {
        for (int i = 0; i < MAXINPUT; i++)
            str[i] = "";
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        Add("切换LOG开关",SwitchLogEnable);
    }

    public void OnChange()
    {
        openLog = !openLog;
    }

    public void Add(string str, CallBack addDelegate)
    {
        mylog.Add(str, new GuiStruct(addDelegate, 0));
    }
    public void Add<T>(string str, CallBack<T> addDelegate)
    {
        mylog.Add(str, new GuiStruct(addDelegate, guiCount));
        guiCount++;
    }
    public void Add<T,U>(string str, CallBack<T,U> addDelegate)
    {
        mylog.Add(str, new GuiStruct(addDelegate, guiCount));
        guiCount += 2;
    }


    private void RunFunc(GuiStruct gs)
    {
        Debug.Log(gs.del.Method.GetParameters().Length);
        switch(gs.del.Method.GetParameters().Length)
        {
            case 0:
                CallBack callback1 = gs.del as CallBack;
                if (callback1 != null)
                    callback1();
                break;
            case 1:
                CallBack<string> callback2 = gs.del as CallBack<string>;
                if (callback2 != null)
                    callback2(str[gs.code]);
                break;
            case 2:
                CallBack<string,string> callback3 = gs.del as CallBack<string,string>;
                if (callback3 != null)
                    callback3(str[gs.code], str[gs.code+1]);
                break;
            default:
                break;
        }
    }

    private void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
        }
        GUI.color = color;

        if (GUI.Button(new Rect(10, screenHeight - 80, 100, 40), "DebugTool"))
        {
            openLog = !openLog;
        }

        if (!openLog)
            return;


        scollPosition = GUILayout.BeginScrollView(scollPosition, GUILayout.Width((float)0.4 * screenWidth), GUILayout.Height((float)0.85 * screenHeight));
        //GUILayout.BeginArea(new Rect(0, screenHeight - 300, 300, 300));
        foreach (KeyValuePair<string,GuiStruct>item in mylog)
        {
            mylog.TryGetValue(item.Key, out gs);
            codeMid = gs.code;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < gs.del.Method.GetParameters().Length; i++)
            {
                str[codeMid] = GUILayout.TextField(str[codeMid], GUILayout.Width(100),GUILayout.Height(30));
                codeMid++;
            }
            if (GUILayout.Button(item.Key, GUILayout.Width(100), GUILayout.Height(30)))
                RunFunc(gs);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUI.color = Color.yellow;
    }

    private void SwitchLogEnable()
    {
        Debug.logger.logEnabled = !Debug.logger.logEnabled;
    }
}
