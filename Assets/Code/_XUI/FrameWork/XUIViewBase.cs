﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class XUIViewBase
{


    public XViewID _id;

    GameObject _root;
    public bool isVisible = false;
    List<Coroutine> taskList = new List<Coroutine>();
    public List<XUIItemBase> itemList = new List<XUIItemBase>();

    public GameObject Root
    {
        get { return _root; }
    }

    public void InitView(GameObject root)
    {
        _root = root;

    }

    public Transform Find(string path)
    {
        return _root.transform.Find(path);
    }

    public Component FindComponent(string path, System.Type obj)
    {
        return Find(path).GetComponent(obj);
    }
    //界面创建后的初始化
    public virtual void OnInit()
    {

    }
    //每次打开后调用
    public virtual void OnShow(object param = null)
    {
        
    }
	//每次关闭后调用
	public virtual void OnHide()
    {

    }

    public virtual void OnDestroy()
    {

    }


    //  public override string SetPrefab()
    // {
    //     return "";
    // }
    // public override string _FullPath()
    // {
    //     return "";
    // }
    public abstract string GetPrefabName();
    public abstract XLayer GetLayer();

    //public abstract string _FullPath();

    public virtual void Update()
    {

    }

    protected void ShowView(XViewID id, bool show, object param = null)
    {
        if (show)
        {
            XEventStruct arg;
            arg.id = id;
            arg.param = param;
            EventManager.Instance.dispatchEvent(EventName.ShowView,arg);
        }
        else
        {
            EventManager.Instance.dispatchEvent(EventName.CloseView, id);
        }
    }

    protected void ShowBlock(bool show)
    {
        if (show)
        {
            EventManager.Instance.dispatchEvent(EventName.ShowBlock);
		}
        else
        {
            EventManager.Instance.dispatchEvent(EventName.CloseBlock);
		}
    }

    protected void CloseSelf()
    {
		EventManager.Instance.dispatchEvent(EventName.CloseView, _id);
	}

    //关闭前/onHide之前
    public virtual void DoHide(System.Action func)
    {
        if (this._root)
            this._root.SetActive(false);

        for (int n = 0; n < taskList.Count; n++)
        {
            taskList[n].Stop();
        }
        taskList.Clear();

        if (func != null)
        {
            func();
        }
    }

    protected Coroutine StartCoroutine(IEnumerator itor)
    {
        Coroutine task = new Coroutine(itor);
        taskList.Add(task);
        return task;
    }

	public T GetChild<T>(string name) where T : MonoBehaviour
	{
        Transform child = Root.transform.Find(name);
		if (child == null)
		{
            Debug.LogError(name + " is not child of " + Root.name);
			return null;
		}

		T temp = child.gameObject.GetComponent<T>();

		if (temp == null)
		{
			Debug.LogError(name + " dosen't has component " + typeof(T));
		}

		return temp;
	}
}
