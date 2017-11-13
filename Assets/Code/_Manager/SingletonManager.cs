using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton manager.挂在一个持久化GameObject下的类
/// </summary>
public class SingletonManager<T> : MonoBehaviour where T:MonoBehaviour {

    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (null == _instance )
            {
                GameObject go =  GameObject.Find( SystemGlobalSetting.SCENE_MANAGER );
                if( go ==null )
                {
                    go = new GameObject();
                    UnityEngine.GameObject.DontDestroyOnLoad(go);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.name = SystemGlobalSetting.SCENE_MANAGER;
                }
                _instance  = go.AddComponent< T >();

            }
            return _instance;
        }
    }

    public virtual void Initialize() { }
    
    public virtual void UnInitialize() { }
	
}
