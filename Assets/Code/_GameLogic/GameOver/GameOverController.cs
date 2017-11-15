/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/15 13:24:24
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : SingletonController<GameOverController> {

    public override void Initialize()
    {
        globalDispatcher.addEventListener(EventName.GameOver, ShowGameOver);
    }

    private void ShowGameOver()
    {
        Debug.Log("GameOver");
        XUIManager.Instance.ShowOrLoadView<GameOverView>(XViewID.GameOverView);
    }

    public void Restart()
    {
        globalDispatcher.dispatchEvent(EventName.ShowBack);
    }

}
