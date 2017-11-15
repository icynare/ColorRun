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
using UnityEngine.UI;
using MyWhole;

public class BackGroundView : XUIViewBase
{
    private Transform frontBG;
    private Transform backBG;
    private Text score;
    private Player person;
    private float width;

    private float _score = 0;


    public override void OnInit()
    {
        frontBG = Root.FindExt("Wall/FrontBG").transform;
        backBG = Root.FindExt("Wall/BackBG").transform;
        score = Root.GetChildComponet<Text>("Score");
        person = Root.GetChildComponet<Player>("Person");
        width = Screen.width;
    }

    public override void OnShow(object param = null)
    {
        Debug.Log("BackGroundView OnShow");
    }

    public void Reset()
    {
        frontBG.localPosition = Vector3.zero;
        backBG.localPosition = new Vector3(width, backBG.localPosition.y, backBG.localPosition.z);
        _score = 0;
        person.Reset();
    }

    public override void Update()
    {
        MakePosition();
        MakePoint();
        frontBG.localPosition = new Vector3(frontBG.localPosition.x - Time.deltaTime * WholeData.Back_Speed, frontBG.localPosition.y,
            frontBG.localPosition.z);
        backBG.localPosition = new Vector3(backBG.localPosition.x - Time.deltaTime * WholeData.Back_Speed, backBG.localPosition.y,
            backBG.localPosition.z);
    }

    private void MakePosition()
    {
        if (frontBG.localPosition.x < -width)
            frontBG.localPosition = new Vector3(width, 0, 0);
        if (backBG.localPosition.x < -width)
            backBG.localPosition = new Vector3(width, 0, 0);
    }

    private void MakePoint()
    {
        _score += Time.deltaTime * WholeData.Back_Speed / 100;
        score.text = String.Format("已跑：<color=#FF3030>{0}</color>米", (int)_score);
    }

    public void SpaceDown()
    {
        if (person == null)
            return;
        person.Jump();
        //Debug.Log("跳跃");
    }

    public void Eat(GoodsType type, MyColor color)
    {
        if (person == null)
            return;
        person.Eat(type, color);
        WholeData.curColor = color;
    }

    public void TouchStop(MyColor color)
    {
        if (person == null)
            return;
        if (color == WholeData.curColor)
        {
            Debug.Log("颜色一样");
        }
        else
        {
            BackController.Instance.ShowGameOver();
            person.TouchStop(color);
        }
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
