/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/13 14:26:37
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWhole;
using Random = UnityEngine.Random;

public class MapView : XUIViewBase
{

    private GameObject goods;
    private GameObject stopper;
    private GameObject upStop;
    private GameObject downStop;

    public override void OnInit()
    {
        goods = Root.FindExt("Goods");
        stopper = Root.FindExt("Stopper");
        upStop = stopper.FindExt("Up");
        downStop = stopper.FindExt("Down");

    }

    public override void Update()
    {
        Test();
    }

    public void Reset()
    {
        for (int i = 0; i < goods.transform.childCount; i++)
            goods.transform.GetChild(i).gameObject.GetComponent<GoodsBase>().DestorySelf();
        for (int i = 0; i < upStop.transform.childCount; i++)
            upStop.transform.GetChild(i).gameObject.GetComponent<StopBase>().DestorySelf();
        for (int i = 0; i < downStop.transform.childCount; i++)
            downStop.transform.GetChild(i).gameObject.GetComponent<StopBase>().DestorySelf();
    }

    public void AddGoods(bool isBig)
    {
        if (isBig)
        {
            AddGoods("BigGoods", WholeData.BIGGOODS);
        }
        else
        {
            AddGoods("SmallGoods", WholeData.SMALLGOODS);
        }
    }

    private void AddGoods(string name, string path)
    {
        MyColor color = WholeData.GetRandomColor();
        GameObject obj = MyPoolManager.GetOne(name, ResourceManager.Instance.LoadPrefab(path) as GameObject);
        obj.transform.SetParent(goods.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        GoodsBase temp = obj.GetComponent<GoodsBase>();
        if (temp != null)
        {
            temp.SetColor(color);
        }
    }

    public void AddStop(bool isUp)
    {
        //Debug.Log(">>>AddStopper");
        if (isUp)
        {
            AddStop("Stopper", WholeData.STOPPER, upStop);
        }
        else
        {
            AddStop("Stopper", WholeData.STOPPER, downStop);
        }
    }

    private void AddStop(string name, string path, GameObject parent)
    {
        MyColor color = WholeData.GetRandomColor();
        GameObject obj = MyPoolManager.GetOne(name, ResourceManager.Instance.LoadPrefab(path) as GameObject);
        obj.transform.SetParent(parent.transform);
        obj.transform.localScale = Vector3.one;
        int myran = Random.Range(-50, 100);
        obj.transform.localPosition = new Vector3(0, myran, 0);
        StopBase temp = obj.GetComponent<StopBase>();
        if (temp != null)
        {
            temp.SetColor(color);
        }
    }

    public void AddRainBow()
    {

    }

    public void Test()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Debug.Log(">>>触摸按下");
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Debug.Log("触摸滑动中----");
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Debug.Log("触摸离开<<<");
            }
        }
    }

    public override string GetPrefabName()
    {
        return "Maps/MapView";
    }

    public override XLayer GetLayer()
    {
        return XLayer.secondLayer;
    }

}
