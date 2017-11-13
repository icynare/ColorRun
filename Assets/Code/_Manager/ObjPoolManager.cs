using UnityEngine;
using System.Collections.Generic;

public class ObjPoolManager : Singleton<ObjPoolManager>{
    private Dictionary<int, ObjectPool> poolList = new Dictionary<int, ObjectPool>();

    public void initPool(System.Type objType,object[] cArgs, int minNum, int maxNum)
    {
        int code = objType.GetHashCode();
        if (poolList.ContainsKey(code))
        {
            return;
        }else
        {
            ObjectPool buffPool = new ObjectPool();
            buffPool.Initialize(objType, cArgs, minNum, maxNum);
            poolList.Add(code, buffPool);
        }
    }

    //for test
    public void PrintAllPool()
    {
        Dictionary<int, ObjectPool>.Enumerator ie = poolList.GetEnumerator();
        while(ie.MoveNext())
        {
            Debug.Log("ObjectPool:" + ie.Current);
        }
    }

    public void DisposePool(System.Type objType)
    {
        int code = objType.GetHashCode();
        ObjectPool buffPool;
        if (poolList.TryGetValue(code, out buffPool))
        {
            buffPool.Dispose();
            buffPool = null;
            poolList.Remove(code);
        }
    }

    /// <summary>
    /// 销毁对象
    /// </summary>
    public void DestroyObject(IPooledObjSupporter obj)
    {
        ObjectPool buffPool;
        if(poolList.TryGetValue(obj.GetType().GetHashCode(), out buffPool))
        {
            buffPool.GiveBackObject(obj);
        }
    }
    
    public T CreateObject<T>()
    {
        ObjectPool buffPool;
        if(poolList.TryGetValue(typeof(T).GetHashCode(), out buffPool))
        {
            return (T)buffPool.GetObject();
        }
        return default(T);
    }
}
