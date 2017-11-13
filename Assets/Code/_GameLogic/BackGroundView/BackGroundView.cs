/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/12 17:07:11
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackGroundView : XUIViewBase
{
    public float moveSpeed = 200.0f;
    private Transform frontBG;
    private Transform backBG;
    private Player person;
    private float width;


    public override void OnInit()
    {
        frontBG = Root.FindExt("Wall/FrontBG").transform;
        backBG = Root.FindExt("Wall/BackBG").transform;
        person = Root.GetChildComponet<Player>("Person");
        width = Screen.width;
    }

    public override void Update()
    {
        MakePosition();
        frontBG.localPosition = new Vector3(frontBG.localPosition.x - Time.deltaTime * moveSpeed, frontBG.localPosition.y,
            frontBG.localPosition.z);
        backBG.localPosition = new Vector3(backBG.localPosition.x - Time.deltaTime * moveSpeed, backBG.localPosition.y,
            backBG.localPosition.z);
    }

    private void MakePosition()
    {
        if (frontBG.localPosition.x < -width)
            frontBG.localPosition = new Vector3(width, 0, 0);
        if (backBG.localPosition.x < -width)
            backBG.localPosition = new Vector3(width, 0, 0);
    }

    public void SpaceDown()
    {
        if (person == null)
            return;
        person.Jump();
    }

    public override XLayer GetLayer()
    {
        return XLayer.secondLayer;
    }

    public override string GetPrefabName()
    {
        return "Home/BackGroundView";
    }

}
