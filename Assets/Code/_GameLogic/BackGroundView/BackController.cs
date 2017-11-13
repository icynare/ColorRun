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

public class BackController : SingletonController<BackController>
{
    private BackGroundView backView;

    public override void Initialize()
    {
        globalDispatcher.addEventListener(EventName.ShowBack, ShowBackGround);
        globalDispatcher.addEventListener<float>(EventName.GameLoop, GameLoop);
        globalDispatcher.addEventListener(EventName.SpaceDown, SpaceDown);
    }

    private void ShowBackGround()
    {
        Debug.Log("ShowBackGround");
        backView = XUIManager.Instance.ShowOrLoadView<BackGroundView>(XViewID.BackGroundView);
    }

    private void GameLoop(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Space))
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
