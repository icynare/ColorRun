/*********************************************************************
* Author：ChenKaiBin 
* Mail：ChenKaiBin@talkmoney.cn
* CreateTime：2017/11/12 17:14:02
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyWhole
{
    public enum MyColor
    {
        RED,
        ORANGE,
        YELLO,
        GREEN,
        BLUE,
        PURPLE,
        PINK
    }

    public enum GoodsType
    {
        BIG,
        SMALL,
        RAINBOW
    }

    public enum StopperType
    {
        UP,
        DOWN
    }

    public class WholeData
    {
        public static MyColor curColor;
        public static int curScore;

        public static float Back_Speed = 200.0f;    //背景移动速度
        public static float Junm_High = 300f;   //跳跃高度
        public static float Jump_Time = 0.5f;
        public static float Drop_Time = 0.3f;



        public static float BigGood_Time = 2f;
        public static float SmallGood_Time = 5f;
        public static float RainBow_Time = 20f;
        public static float UpStop_Time = 6f;
        public static float DownStop_Time = 3f;

        public static float Dissaper_Time = 15f;

        public static int ColorNum = 7;
        public static int GoodsNum = 3;
        public static int StopperNum = 2;

        public static float BigSpeed = 300f;
        public static float SmallSpeed = 250f;
        public static float RainSpeed = 400f;

        public static float StopperSpeed = 350f;

        public static float Border = -1600f;

        public static string BIGGOODS = "Maps/BigGoods";
        public static string SMALLGOODS = "Maps/SmallGoods";
        public static string RAINBOW = "Maps/RainBow";
        public static string STOPPER = "Maps/Stopper";

        public static GoodsType GetRandomGood()
        {
            GoodsType[] goods = Enum.GetValues(typeof(GoodsType)) as GoodsType[];
            int kind = Random.Range(0, GoodsNum);
            return goods[kind];
        }

        public static MyColor GetRandomColor()
        {
            MyColor[] colors = Enum.GetValues(typeof(MyColor)) as MyColor[];
            int kind = Random.Range(0, ColorNum);
            return colors[kind];
        }

        public static StopperType GetRandomStopper()
        {
            StopperType[] stops = Enum.GetValues(typeof(StopperType)) as StopperType[];
            int kind = Random.Range(0, StopperNum);
            return stops[kind];
        }

        public static Color GetColorStatus(MyColor color)
        {
            switch (color)
            {
                case MyColor.RED:
                    return Color.red;
                case MyColor.ORANGE:
                    return Color.cyan;
                case MyColor.YELLO:
                    return Color.yellow;
                case MyColor.GREEN:
                    return Color.green;
                case MyColor.BLUE:
                    return Color.blue;
                case MyColor.PURPLE:
                    return Color.gray;
                case MyColor.PINK:
                    return Color.magenta;
                default:
                    return Color.black;
            }
        }

        public static float GetTypeFill(GoodsType type)
        {
            if (type == GoodsType.BIG)
            {
                return Dissaper_Time/5;
            }
            if (type == GoodsType.SMALL)
            {
                return Dissaper_Time/10;
            }
            else
                return 0;
        }

    }

}


