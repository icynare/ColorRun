/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/14 17:46:29
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using MyWhole;
using UnityEngine;
using UnityEngine.UI;

public class StopBase : MonoBehaviour
{

    protected MyColor _color;
    protected string _name;
    protected float _moveSpeed = 0;

    private float border = WholeData.Border;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (transform.localPosition.x < border)
	        DestroyObject(gameObject);
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
            MapController.Instance.TouchStop(GetColor());
            DestroyObject(gameObject);
        }
    }

    public void SetColor(MyColor color)
    {
        _color = color;
        Image me = gameObject.GetComponent<Image>();
        me.color = WholeData.GetColorStatus(_color);
    }

    public MyColor GetColor()
    {
        return _color ;
    }

}
