/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/14 10:15:35
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyShortCut {

    //在路径后面空格然后可以添加快捷键 %代表Ctrl #代表Shift &代表Alt _加字母表示单独某个字母
    [MenuItem("GameObject/SetActive %q", false, 10)]
    static void MySetActive()
    {
        GameObject[] selections = Selection.gameObjects;
        foreach (GameObject go in selections)
        {
            go.SetActive(!go.activeSelf);
        }
    }
}
