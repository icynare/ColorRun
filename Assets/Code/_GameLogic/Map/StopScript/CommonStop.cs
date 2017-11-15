/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/14 18:58:34
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonStop : StopBase {

    public CommonStop()
    {
        _name = "Stopper";
        _moveSpeed = MyWhole.WholeData.StopperSpeed;
    }
	
}
