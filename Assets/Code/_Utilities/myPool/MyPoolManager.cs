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

public class MyPoolManager {

	public const float Alive_time = 60;
    public static Dictionary<string, MyPoolGroup> poolList = new Dictionary<string, MyPoolGroup>();

    public static GameObject GetOne(string name, GameObject obj)
    {
        MyPoolGroup mypool;
        if (!poolList.ContainsKey(name))
        {
            Debug.Log(">>>PoolName" + name);
            mypool = new MyPoolGroup(obj);
            poolList.Add(name, mypool);
        }
        mypool = poolList[name];
        return mypool.GetObj();
    }

    public static void RemoveOne(string name, GameObject obj)
    {
        MyPoolGroup mypool;
        poolList.TryGetValue(name, out mypool);
        if (mypool != null)
        {
            mypool.RemoveObj(obj);
        }
    }

    public static void DestoryAll()
    {
        foreach (MyPoolGroup pool in poolList.Values)
            pool.DestoryAll();
    }

    public static void CheckOver()
    {
        foreach (MyPoolGroup pool in poolList.Values)
            pool.CheckOverTimer();
    }

    public static int GetNum(string name, GameObject obj)
    {
        MyPoolGroup mypool;
        poolList.TryGetValue(name, out mypool);
        if (mypool != null)
            return mypool.GetNum();
        return 0;
    }
}
