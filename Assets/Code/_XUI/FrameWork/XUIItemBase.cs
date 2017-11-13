using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class XUIItemBase
{

    GameObject parentObj;
    XUIViewBase parentView;
    string prefabPath;
    bool needPool;
    private Vector3 pos;

    GameObject _root;
    private bool _visible = true;
    List<Coroutine> taskList = new List<Coroutine>();

    //parentObj：挂在哪里
    //parentView
    //needPool:是否需要对象池
    public XUIItemBase(GameObject parentObj, XUIViewBase parentView, bool needPool = false)
    {
        this.parentObj = parentObj;
        this.parentView = parentView;
        this.needPool = needPool;
        InitView();

    }

    public bool GetNeedPool()
    {
        return needPool;
    }

    public GameObject Root
    {
        get { return _root; }
    }

    public void InitView()
    {
        GameObject root = XUIManager.Instance.LoadItemPrefab(GetPrefabPath(), parentObj.transform, needPool);
        parentView.itemList.Add(this);
        _root = root;
        OnInit();
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

    public virtual void OnDestroy()
    {
        parentView.itemList.Remove(this);
        Root.transform.parent = null;
        XUIManager.Instance.DestroyItem(this);
    }


    //  public override string SetPrefab()
    // {
    //     return "";
    // }
    // public override string _FullPath()
    // {
    //     return "";
    // }

    //exp:View/Login/LoginItem
    public abstract string GetPrefabPath();


    //XUIItemBase没有update事件
    //public virtual void Update()
    //{

    //}

    protected void CloseSelf()
    {

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


    public bool visible
    {
        get
        {
            return _visible;
        }
        set
        {
            if (value == _visible)
            {
                return;
            }
            else
            {
                Root.SetActive(value);
                _visible = value;
            }
        }


    }



    public void SetPos(float x, float y)
    {
        if (pos == null)
            pos = new Vector3();
        pos.x = x;
        pos.y = y;
        Root.transform.localPosition = pos;
    }

    public Vector2 GetPos()
    {
        return new Vector2(Root.transform.localPosition.x, Root.transform.localPosition.y);
    }

}
