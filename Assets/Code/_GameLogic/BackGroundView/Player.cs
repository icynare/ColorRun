/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/12 22:08:21
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{

    private float high = 200f;
    private float startY = 0;
    bool canJump = true;

    public void Jump()
    {
        if (canJump == false)
            return;
        canJump = false;
        startY = transform.localPosition.y;
        Debug.LogFormat("世界坐标：({0},{1},{2})", this.transform.position.x, this.transform.position.y,
                        this.transform.position.z);
        Debug.LogFormat("本地坐标：({0},{1},{2})", this.transform.localPosition.x, this.transform.localPosition.y,
                        this.transform.localPosition.z);

        this.transform.DOLocalMove(new Vector3(transform.localPosition.x, transform.localPosition.y + high, transform.localPosition.z),0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Debug.Log("Complete");
            this.transform.DOLocalMove(
                new Vector3(transform.localPosition.x, startY,
                    transform.localPosition.z), 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                canJump = true;
            });
        });
    }


}
