using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObjectPool
{
    private System.Type _objType;
    private int _minNum;
    private int _maxNum;

    public Stack<IPooledObjSupporter> list;
    public int size = 0;//一共产生过多少个示例，监控用

    public ObjectPool()
    {
        list = new Stack<IPooledObjSupporter>();
    }
    public void Initialize(System.Type objType, object[] cArgs, int minNum, int maxNum)
    {
        _objType = objType;
        _minNum = minNum;
        _maxNum = maxNum;

    }

    public void Dispose()
    {
       
    }
    public void GiveBackObject(IPooledObjSupporter obj)
    {
        if (obj == null) return;
        list.Push(obj);
    }


    public IPooledObjSupporter GetObject()
    {
        if (list.Count < 1)
        {
            size++;
            return (IPooledObjSupporter)Activator.CreateInstance(_objType);
        }
        IPooledObjSupporter clz = list.Pop();
        clz.Reset();
        return clz;
    }

    
}