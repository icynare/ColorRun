/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/14 14:54:52
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using MyWhole;
using UnityEngine;

public class SmallGoods : GoodsBase {

    public SmallGoods()
    {
        _type = GoodsType.SMALL;
        _moveSpeed = WholeData.SmallSpeed;
    }
}
