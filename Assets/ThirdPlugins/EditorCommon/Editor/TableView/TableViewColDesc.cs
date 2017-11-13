/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/10/17 14:06:02
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace EditorCommon
{
    public class TableViewColDesc
    {
        public string PropertyName;
        public string TitleText;

        public TextAnchor Alignment;
        public string Format;
        public float WidthInPercent;

        public FieldInfo FieldInfo;

        public string FormatObject(object obj)
        {
            return PAEditorUtil.FieldToString(obj, FieldInfo, Format);
        }

        public int Compare(object o1, object o2)
        {
            object fv1 = PAEditorUtil.FieldValue(o1, FieldInfo);
            object fv2 = PAEditorUtil.FieldValue(o2, FieldInfo);

            IComparable fc1 = fv1 as IComparable;
            IComparable fc2 = fv2 as IComparable;
            if (fc1 == null || fc2 == null)
            {
                return fv1.ToString().CompareTo(fv2.ToString());
            }

            return fc1.CompareTo(fc2);
        }
    }
}

