/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/13 14:23:14
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWhole;

public class MapController : SingletonController<MapController>
{
    private float _bigGood = WholeData.BigGood_Time;
    private float _smallGood = 0.1f;
    private float _rainbow = WholeData.RainBow_Time;
    private float _upStop = WholeData.UpStop_Time;
    private float _downStop = 1f;

    private float _lastTime = 0;
    private float _lastStop = 0;

    private MapView mapView;
    private float randomTemp = 0;

    public override void Initialize()
    {
        globalDispatcher.addEventListener<float>(EventName.GameLoop, GameLoop);
        globalDispatcher.addEventListener(EventName.ShowBack, ShowBackGround);
    }

    private void ShowBackGround()
    {
        mapView = XUIManager.Instance.ShowOrLoadView<MapView>(XViewID.MapView);
        mapView.Reset();
        TimerReset();
    }

    private void TimerReset()
    {
        _bigGood = WholeData.BigGood_Time;
        _smallGood = 0.1f;
        _rainbow = WholeData.RainBow_Time;
        _upStop = WholeData.UpStop_Time;
        _downStop = 1f;

        _lastTime = 0;
        _lastStop = 0;
    }

    private void GameLoop(float deltaTime)
    {
        if (mapView == null || !mapView.isVisible)
            return;

        GoodsTimer(deltaTime);
        StopperTimer(deltaTime);
    }

    private void GoodsTimer(float deltaTime)
    {
        _lastTime += deltaTime;
        if (_lastTime < 1.5f)
            return;

        _bigGood -= deltaTime;
        _smallGood -= deltaTime;
        _rainbow -= deltaTime;
        if (_bigGood <= 0)
        {
            _lastTime = 0;
            mapView.AddGoods(true);
            randomTemp = Random.Range(-5, 5);
            _bigGood = randomTemp + WholeData.BigGood_Time;
        }
        if (_smallGood <= 0)
        {
            _lastTime = 0;
            mapView.AddGoods(false);
            randomTemp = Random.Range(-3, 3);
            _smallGood = randomTemp + WholeData.SmallGood_Time;
        }
        if (_rainbow <= 0)
        {
            _lastTime = 0;
            mapView.AddRainBow();
            randomTemp = Random.Range(-5, 5);
            _rainbow = randomTemp + WholeData.RainBow_Time;
        }
    }

    private void StopperTimer(float deltaTime)
    {
        _lastStop += deltaTime;
        if (_lastStop < 1f)
            return;

        _upStop -= deltaTime;
        _downStop -= deltaTime;

        if (_upStop <= 0)
        {
            _lastStop = 0;
            mapView.AddStop(true);
            randomTemp = Random.Range(-5, 5);
            _upStop = randomTemp + WholeData.UpStop_Time;
        }
        if (_downStop <= 0)
        {
            _lastStop = 0;
            mapView.AddStop(false);
            randomTemp = Random.Range(-3, 3);
            _downStop = randomTemp + WholeData.DownStop_Time;
        }
    }

    public void EatGoods(GoodsType type, MyColor color)
    {
        globalDispatcher.dispatchEvent(EventName.EatGoods, type, color);
    }

    public void TouchStop(MyColor color)
    {
        globalDispatcher.dispatchEvent(EventName.TouchStop, color);
    }

}
