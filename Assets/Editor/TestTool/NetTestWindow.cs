﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using Msg;
using UnityEngine;
using UnityEditor;
using Google.Protobuf;

public class NetTestUtility: EditorWindow
{
    //sprivate MsgTypes msgType;
    //private NetConfig config = new NetConfig();
    private List<string> propertyNames = new List<string>();
    private string[] propertyValues = null;
    private PropertyInfo[] propertyInfos = null;
    System.Type type;
    const string NAMESPACE = "Msg";


    private string[] typeNames; 
    private int selectedIndex = 0;
    private List<System.Type> typeList;
    private int code =100;

    //协议测试工具
    [MenuItem("Tools/NetTest")]
    static void OpenWindow()
    {
        NetTestUtility netTestUtility = (NetTestUtility)EditorWindow.GetWindow(typeof(NetTestUtility));

        netTestUtility.InitWindow();


	}

    public void InitWindow()
    {
		Assembly am = Assembly.GetAssembly(typeof(Msg.CAuthLogin));
		Type[] types = am.GetTypes();
		List<string> typeNameList = new List<string>();
        typeList = new List<Type>();


		for (int i = 0; i < types.Length; i++)
		{
            if (types[i].Namespace == NAMESPACE && !types[i].IsNested && types[i].Name.StartsWith("C"))
			{
				typeNameList.Add(types[i].Name);
                typeList.Add( types[i]);
				Debug.Log(types[i].Name);
			}
		}
        typeNames = typeNameList.ToArray();

        selectedIndex = 0;
	}


    void OnGUI()
	{
        if (typeNames.Length <= 0)
        {
            EditorGUILayout.LabelField("获取协议失败");
			return;
        }
           
        //Debug.Log(typeNames);
        selectedIndex = EditorGUILayout.Popup("协议类型:", selectedIndex, typeNames);
		code = EditorGUILayout.IntField("协议号：", code);
        //msgType = (MsgTypes)EditorGUILayout.EnumPopup("协议类型：", msgType);
	    if (GUILayout.Button("确定"))
	    {
            GetProperties();
	    }
   
	    if (0 < propertyNames.Count)
	    {
            LoadProperties();
            if (GUILayout.Button("发送"))
            {
                Debug.Log("发送协议到服务器");
                SendMessage();
            }
        }
	    else
	    {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("无效协议类型");
            EditorGUILayout.EndHorizontal();
        }

    }


    //反射属性
    void GetProperties()
    {
        propertyNames.Clear();
        Debug.Log("反射协议内容");
        //Debug.Log("选择的类型： " + msgType);
        Type type = typeList[selectedIndex];
        Debug.Log($"选择的类型：{type}");
        if (type != null)
        {
            Debug.Log(type);
            propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].Name == "Parser" || propertyInfos[i].Name == "Descriptor")
                    continue;
                if (propertyInfos[i].PropertyType.IsArray)
                {
                    Debug.Log("数组类型： " + propertyInfos[i].PropertyType.GetElementType());
                }
                if (propertyInfos[i].PropertyType.IsClass)
                {
                    Debug.Log("内部类： " + propertyInfos[i].PropertyType);
                }
                propertyNames.Add(propertyInfos[i].Name);
            }
            propertyValues = new string[propertyNames.Count];
        }
        else
        {
            Debug.Log("找不到对应类型");
        }
    }


    //加载属性面板
    void LoadProperties()
    {
        for (int i = 0; i < propertyNames.Count; i++)
        {
            CreatInputField(propertyNames[i], ref propertyValues[i]);
        }
    }

    //画出一个属性的标签和输入框
    void CreatInputField(string name, ref string value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();
    }

    //发送协议
    void SendMessage()
    {
        var obj = GetEntity();
        //Convert.ChangeType(obj, type, null);
        //MSG.Person p = (MSG.Person) obj;

        if (null != obj)
        {
            //Debug.Log("类型转换： " + obj.GetType());
            Debug.Log($"类型转换：{obj.GetType()}");
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                Debug.Log(propertyInfos[i].GetValue(obj, null));
            }
            Debug.Log($"发送协议{obj}");
            NetManager.Instance.SendMsg((ushort)code, obj as IMessage);

        }
        else
        {
            Debug.Log("没有创建一个新对象");
        }
    }

    //反射生成一个新对象并赋值
    object GetEntity()
    {
        Type type = typeList[selectedIndex];
        //Debug.Log("生成的类型： " + type);
        Debug.Log($"生成的类型：{type}");
        object obj = System.Activator.CreateInstance(type);
        //Debug.Log("属性数量： " + propertyInfos.Length +
        //    "\n属性名数量： " + propertyNames.Count +
        //    "\n属性值数量： " + propertyValues.Length);
        Debug.Log($"属性数量：{propertyInfos.Length}\n" +
                  $"属性名数量：{propertyNames.Count}\n" +
                  $"属性值数量：{propertyValues.Length}");
        if (null != propertyInfos)
        {
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                for (int j = 0; j < propertyNames.Count; j++)
                {
                    if (propertyInfos[i].Name == propertyNames[j])
                    {
                        Type propertyType = propertyInfos[i].PropertyType;
                        propertyInfos[i].SetValue(obj, Convert.ChangeType(propertyValues[j], propertyType), null);
                        propertyType.GetElementType();
                    }


                }
            }
        }

        return obj;
    }
    void OnDestroy()
    {
        propertyNames.Clear();
        propertyValues = null;
        propertyInfos = null;
    }
}

