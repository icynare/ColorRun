/*********************************************************************
* Autor：xiaozhang 
* Mail：mobile@talkmoney.cn
* CreateTime：2017/10/30 11:46:29
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPoolGroup
{
    private Dictionary<int, MyPoolItem> itemList;
    //private List<MyPoolItem> itemList;
    private string _name;
    private GameObject _myobj;

    public MyPoolGroup(GameObject prefab)
    {
        _name = prefab.name;
        _myobj = prefab;
        itemList = new Dictionary<int, MyPoolItem>();
    }

    public string GetName()
    {
        return _name;
    }

    public GameObject GetObj()
    {
        GameObject obj = null;
        if (itemList == null)
            itemList = new Dictionary<int, MyPoolItem>();
        foreach (KeyValuePair<int,MyPoolItem> item in itemList)
        {
            if (!item.Value.isActive)
            {
                obj = item.Value.GetObj();
                break;
            }
        }
        if (obj == null)
        {
            obj = Object.Instantiate(_myobj);
            itemList.Add(obj.GetHashCode(),new MyPoolItem(obj));
        }
        return obj;
    }

    public void RemoveObj(GameObject obj)
    {
        int index = obj.GetHashCode();
        if (itemList.ContainsKey(index))
        {
            itemList[index].Destory();
        }
    }

    public void DestoryObj(GameObject obj)
    {
        int index = obj.GetHashCode();
        if (itemList.ContainsKey(index))
        {
            Object.Destroy(obj);
            itemList.Remove(index);
        }
    }

    public void DestoryAll()
    {
        foreach (KeyValuePair<int, MyPoolItem>item in itemList)
        {
            Object.Destroy(item.Value.GetObj());
            itemList.Remove(item.Key);
        }
    }

    public void CheckOverTimer()
    {
        foreach (KeyValuePair<int, MyPoolItem>item in itemList)
        {
            if (item.Value.IsOverTime())
            {
                Object.Destroy(item.Value.GetObj());
                itemList.Remove(item.Key);
            }
        }
    }

    public int GetNum()
    {
        if (itemList != null)
            return itemList.Count;
        return 0;
    }

}
