/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/13 22:33:42
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWhole;
using UnityEngine.UI;

public class GoodsBase : MonoBehaviour
{
    protected MyColor _color;
    protected GoodsType _type;
    protected string _name;
    protected float _moveSpeed = 0;

    private float border = WholeData.Border;

    public void  Start()
    {

    }

    public void Update()
    {
        if (transform.localPosition.x < border)
            DestroyObject(gameObject);
        //MyPoolManager.RemoveOne(_name, gameObject);
        else
            transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime * _moveSpeed,
                transform.localPosition.y, transform.localPosition.z);
    }

    public void DestorySelf()
    {
        DestroyObject(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(GetTheType());
            Debug.Log(GetColor());
            MapController.Instance.EatGoods(GetTheType(), GetColor());
            DestroyObject(gameObject);
        }
    }

    public void SetColor(MyColor color)
    {
        _color = color;
        //Debug.Log("颜色：" + _color);
        //Debug.Log(GetColor());
        Image me = gameObject.GetComponent<Image>();
        me.color = WholeData.GetColorStatus(_color);
    }

    public MyColor GetColor()
    {
        return _color;
    }

    public GoodsType GetTheType()
    {
        return _type;
    }

}
