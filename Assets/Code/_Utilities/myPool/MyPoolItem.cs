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

public class MyPoolItem
{
    private GameObject _gameobject;
    private float destoryTime;
    public bool isActive;

    public MyPoolItem(GameObject obj)
    {
        _gameobject = obj;
        destoryTime = 0;
        isActive = true;
    }

    public GameObject GetObj()
    {
        _gameobject.SetActive(true);
        isActive = true;
        destoryTime = 0;
        return _gameobject;
    }

    public void Destory()
    {
        _gameobject.SetActive(false);
        isActive = false;
        destoryTime = Time.time;
    }

    public bool IsOverTime()
    {
        if (isActive)
            return false;
        if (Time.time - destoryTime > MyPoolManager.Alive_time)
            return true;
        return false;
    }

}
