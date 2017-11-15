/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/15 13:10:14
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : XUIViewBase
{

    private Button restart;
    private Button quit;
    private Text score;

    public override void OnInit()
    {
        restart = Root.GetChildComponet<Button>("Restart");
        quit = Root.GetChildComponet<Button>("Quit");
        score = Root.GetChildComponet<Text>("Score");

        restart.onClick.AddListener(() =>
        {
            GameOverController.Instance.Restart();
            XUIManager.Instance.HideView(XViewID.GameOverView);
            Time.timeScale = 1;
        });
        quit.onClick.AddListener(() =>
        {

        });
    }

    public override void OnShow(object param = null)
    {
        score.text = MyWhole.WholeData.curScore.ToString();
    }

    public override XLayer GetLayer()
    {
        return XLayer.topLayer;
    }

    public override string GetPrefabName()
    {
        return "Over/GameOverView";
    }

}
