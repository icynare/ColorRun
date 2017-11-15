/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/12 22:08:21
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MyWhole;

public class Player : MonoBehaviour
{
    private Image me;

    private float startY = 0;
    int canJump = 0;
    private float nowFill = WholeData.Dissaper_Time;

    private float dropTime = 0;

    Tween tween;

    public void Awake()
    {
        startY = transform.localPosition.y;
        me = GetComponent<Image>();
    }

    public void Reset()
    {
        me.fillAmount = 1;
        nowFill = WholeData.Dissaper_Time;
        me.color = Color.white;
    }

    public void Jump()
    {
        //Debug.Log("跳跃");
        //isJump = true;
        //if (canJump >= 2)
        //    return;
        //canJump++;
        //time += Time.deltaTime;
        //tempY = v0 * time - g * (float)Math.Pow(time, 2) / 2;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + tempY,
        //    transform.localPosition.z);
        //if (tempY == 0)
        //{
        //    canJump = true;
        //}

        if (canJump == 2)
            return;
        canJump++;
        if (canJump == 2 && (tween != null && tween.IsPlaying()))
        {
            tween.Kill();
        }
        tween = transform.DOLocalMove(new Vector3(transform.localPosition.x, transform.localPosition.y + WholeData.Junm_High, transform.localPosition.z), WholeData.Jump_Time).SetEase(Ease.OutQuad).OnComplete(() =>
         {
             Debug.Log("Complete: " + transform.localPosition.y);
             dropTime = WholeData.Drop_Time * (float)Math.Pow((transform.localPosition.y-startY)/WholeData.Junm_High,0.5f);
             Debug.Log(dropTime);
             tween = transform.DOLocalMove(
                 new Vector3(transform.localPosition.x, startY,
                     transform.localPosition.z), dropTime).SetEase(Ease.InQuad).OnComplete(() =>
                 {
                     canJump = 0;
                 });
         });

    }

    public void Eat(GoodsType type, MyColor color)
    {
        me.color = WholeData.GetColorStatus(color);
        nowFill += WholeData.GetTypeFill(type);
        nowFill = Mathf.Clamp(nowFill, 0, WholeData.Dissaper_Time);
    }

    public void TouchStop(MyColor color)
    {
        Debug.Log("You are Died");
        Time.timeScale = 0;
    }

    public void Update()
    {
        nowFill -= Time.deltaTime;
        nowFill = Mathf.Clamp(nowFill, 0, WholeData.Dissaper_Time);
        //Debug.Log(nowFill);
        me.fillAmount = nowFill / WholeData.Dissaper_Time;
    }

}
