﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XUIManager:Singleton<XUIManager>
{
    XUIConfig config;
    Dictionary<XViewID, XUIViewBase> viewPool = new Dictionary<XViewID, XUIViewBase>();
    Transform _uiRoot;
    Transform[] layers;
    Transform layerGuide;
    GameObject block;

    GraphicRaycaster[] GRS;

    public delegate void UpdateFunc();
    public UpdateFunc OnUpdateHandle;


    public void Init()
    {
        _uiRoot = GameObject.Find("Canvas").transform;
        config = new XUIConfig();
        layers = new Transform[4];
        GRS = new GraphicRaycaster[4];
        for (int n = 0; n < _uiRoot.transform.childCount; n++)
        {
            Transform child = _uiRoot.transform.GetChild(n);
            layers[n] = child;
            GraphicRaycaster gr = child.GetComponent<GraphicRaycaster>();
            GRS[n] = gr;
        }

		//todo:Block view
		block = _uiRoot.gameObject.FindExt("TopLayer/Block");
        layerGuide = GameObject.Find("CanvasGuide").transform;

        EventManager.Instance.addEventListener<float>(EventName.GameLoop, Update);

    }

    public List<RaycastResult> DoLayerRaycast(PointerEventData data)
    {
        List<RaycastResult> total = new List<RaycastResult>();
        for (int n = 0; n < GRS.Length; n++)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            GRS[n].Raycast(data, results);
            if (results.Count > 0)
            {
                total.AddRange(results);
            }
        }
        return total;
    }

    public void ShowBlock()
    {
        block.SetActive(true);
    }

    public void CloseBlock()
    {
        block.SetActive(false);
    }


    public T ShowOrLoadView<T>(XViewID id,object param = null) where T:XUIViewBase
    {
		XUIViewBase view = null;
		if (!viewPool.TryGetValue(id, out view) || viewPool[id] == null)
		{
			System.Type _type;
			if (config.viewType.TryGetValue(id, out _type))
			{
				XUIViewBase obj = System.Activator.CreateInstance(typeof(T)) as XUIViewBase;
				view = obj;

				view.InitView(TrackRoot(view, id));
				viewPool.Add(id, view);
				view._id = id;
				SetActive(view.Root, true);
				view.OnInit();
				//CheckGroup(view.Group, view._id);
			}
			else
				Debug.LogErrorFormat("Must regest first: {0}", id.ToString());

		}

		if (!view.isVisible)
		{
			OnUpdateHandle += view.Update;
			if (SetActive(view.Root, true))
			{
				view.isVisible = true;
				//CheckGroup(view.Group, view._id);
				view.OnShow(param);
			}
			else
				Debug.LogErrorFormat("This view init failed! :{0}", id.ToString());
		}
        return view == null ? null : view as T;

    }

    //path exp:View/Login/loginItem
    public GameObject LoadItemPrefab(string path,Transform parent, bool needPool = false)
    {
		Object prefab = ResourceManager.Instance.LoadPrefab(path);
		GameObject instance = Object.Instantiate(prefab) as GameObject;
        if (needPool)
        {
            instance = MyPoolManager.GetOne(prefab.name, (GameObject)prefab);
            //Debug.Log(prefab.name + "<color=#111111>>>>></color>" + MyPoolManager.GetNum(prefab.name, (GameObject)prefab));
        }
        else
            instance = Object.Instantiate(prefab) as GameObject;
        instance.name = prefab.name;

        instance.transform.SetParent(parent);
		instance.transform.localPosition = Vector3.zero;
		instance.transform.localScale = Vector3.one;
		return instance;
    }

    public void HideView(XViewID id)
    {
		XUIViewBase view = null;
        if (!viewPool.TryGetValue(id, out view) || viewPool[id] == null)
        {
            return;
        }
		if (view.isVisible)
		{
			view.isVisible = false;
			view.DoHide(() =>
			{
				view.OnHide();
				OnUpdateHandle -= view.Update;
			});

		}

    }

    public void HideAll()
    {
        //todo:
        XUIViewBase view = null;
        if(viewPool == null)
        {
            return;
        }
        foreach(KeyValuePair<XViewID,XUIViewBase> kv in viewPool)
        {
            view = kv.Value;
            if(view.isVisible)
            {
                view.isVisible = false;
                view.DoHide(() =>
                {
                    view.OnHide();
                    OnUpdateHandle -= view.Update;
                });
            }
        }
    }




    bool SetActive(GameObject root, bool active)
    {
        if (root == null)
            return false;
        else
        {
            root.SetActive(active);
            return true;
        }
    }

    public void ClearAll()
    {
        Dictionary<XViewID, XUIViewBase>.Enumerator iter = viewPool.GetEnumerator();
        while (iter.MoveNext())
        {
            XUIViewBase view = iter.Current.Value;
            if (view._id != XViewID.AlertView && view._id != XViewID.ConfirmView)
            {
                view.OnHide();
                DestroyView(view._id);
            }

        }
        viewPool.Clear();
    }

    public void DestroyItem( XUIItemBase item)
    {
        if (!item.GetNeedPool())
            DestoryItemTrue(item);
        else
        {
            MyPoolManager.RemoveOne(item.Root.name, item.Root);
        }
    }

    public void DestoryItemTrue(XUIItemBase item)
    {
        if (item.Root)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(item.Root);
            }
            else
            {
                Object.DestroyImmediate(item.Root);
            }
        }
    }

    void DestroyView(XViewID id)
    {
        XUIViewBase view;
        if (viewPool.TryGetValue(id, out view))
        {
            OnUpdateHandle -= view.Update;
            view.OnDestroy();
            Object.DestroyImmediate(view.Root);
            Resources.UnloadUnusedAssets();
            viewPool.Remove(id);
        }
    }

    GameObject TrackRoot(XUIViewBase view, XViewID id)
    {

        Transform parent;
        if (view.GetLayer() == XLayer.guide)
            parent = layerGuide;
        else
            parent = layers[(int)view.GetLayer()];

        // if (parent == null)
        // {
        if (!string.IsNullOrEmpty(view.GetPrefabName()))
        {
            string path = string.Format("{0}", view.GetPrefabName());
            Object prefab = ResourceManager.Instance.LoadPrefab(path);
            GameObject instance = Object.Instantiate(prefab) as GameObject;

            instance.name = prefab.name;

            instance.transform.SetParent(parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            return instance;
        }
        else
            Debug.LogErrorFormat("Undefined _PrefabName(): {0}", view._id.ToString());
        //}

        return null;
    }

    //本项目不采用group机制
    void CheckGroup(XGroup group, XViewID id)
    {
        // if (group == XGroup.none) return;
        // Dictionary<XViewID, XUIViewBase>.Enumerator iter = viewPool.GetEnumerator();
        // while (iter.MoveNext())
        // {
        //     XUIViewBase view = iter.Current.Value;
        //     if (view == null || view._id == id) continue;
        //     if (view.Group == group && view.isVisible)
        //         ShowView(view._id, false);
        // }
    }

    public void Update(float delta)
    {
        if (OnUpdateHandle != null)
            OnUpdateHandle();
    }

    public override void UnInitialize()
    {
        EventManager.Instance.removeEventListener<float>(EventName.GameLoop, Update);
    }

}


