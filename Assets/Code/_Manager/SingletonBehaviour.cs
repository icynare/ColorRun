
// **********************************************************************
using UnityEngine;
using System;
using System.Collections.Generic;
/***********************************************************************/

public class SingletonBehaviour<S> : MonoBehaviour where S : MonoBehaviour
{
    private static S mSingleton;

    public virtual void Awake()
    {
        mSingleton = (S)(MonoBehaviour)this;
    }
    public static S GetInstance()
    {
        return mSingleton;
    }

}
