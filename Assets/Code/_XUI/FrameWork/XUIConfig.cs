using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XUIConfig
{

    public Dictionary<XViewID, System.Type> viewType = new Dictionary<XViewID, System.Type>();

    public XUIConfig()
    {
        viewType.Add(XViewID.BackGroundView, typeof(BackGroundView));

    }
}

//在此处加一个id 2/3
public enum XViewID
{
    none,
    AlertView,
    ConfirmView,

    BackGroundView,
    
}

//按需加group 3/3
public enum XGroup
{
    none,
    normal,
}

public enum XLayer
{
    firstLayer,
    secondLayer,
    thirdLayer,
    topLayer,
    guide,
}

public struct XEventStruct
{
    public XViewID id;
    public object param;
}
