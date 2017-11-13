/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/09 17:42:18
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDateTimeNow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Debug.Log(ServerTimeManager.nowUnixMS());
        Debug.Log(ServerTimeManager.nowMS());
	}
	
	// Update is called once per frame
	void Update () {

	}
}
