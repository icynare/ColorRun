/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/12 22:14:41
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWhole;

public class BackController : SingletonController<BackController>
{
    private BackGroundView backView;

    public override void Initialize()
    {
        globalDispatcher.addEventListener(EventName.ShowBack, ShowBackGround);
        globalDispatcher.addEventListener<float>(EventName.GameLoop, GameLoop);
        globalDispatcher.addEventListener(EventName.SpaceDown, SpaceDown);
        globalDispatcher.addEventListener<GoodsType, MyColor>(EventName.EatGoods,EatSomething);
        globalDispatcher.addEventListener<MyColor>(EventName.TouchStop, TouchStop);
    }

    private void ShowBackGround()
    {
        backView = XUIManager.Instance.ShowOrLoadView<BackGroundView>(XViewID.BackGroundView);
        backView.Reset();
    }

    private void EatSomething(GoodsType type, MyColor color)
    {
        if (backView == null && backView.isVisible)
            return;

        Debug.Log("吃到了东西");
        backView.Eat(type, color);
    }

    private void TouchStop(MyColor color)
    {
        if (backView == null && backView.isVisible)
            return;

        Debug.Log("撞到了东西");
        backView.TouchStop(color);
    }

    public void ShowGameOver()
    {
        globalDispatcher.dispatchEvent(EventName.GameOver);
    }

    private void GameLoop(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) 
            || Input.touchCount == 1)
        {
            globalDispatcher.dispatchEvent(EventName.SpaceDown);
        }
    }

    private void SpaceDown()
    {
        if (backView == null && backView.isVisible)
            return;
        backView.SpaceDown();
    }

}
