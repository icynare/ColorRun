using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 事件管理器
/// </summary>
public class EventManager : SingletonEventDispatcher<EventManager>
{
    private ObjectPool m_eventPool = null;
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Initialize()
    {
        m_eventPool = new ObjectPool();
        m_eventPool.Initialize(typeof(GameEvent), null, 20, 10000);
    }

    /// <summary>
    /// 析构
    /// </summary>
    public override void UnInitialize()
    {
        m_eventPool.Dispose();
    }

    /// <summary>
    /// 创建事件
    /// </summary>
    public GameEvent CreateEvent()
    {
        return (m_eventPool.GetObject() as GameEvent);
    }

    /// <summary>
    /// 销毁事件
    /// </summary>
	/// 

    public void DestroyEvent(GameEvent gameEvent)
    {
        m_eventPool.GiveBackObject(gameEvent);
    }
}