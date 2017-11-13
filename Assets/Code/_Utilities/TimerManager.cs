using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 定时管理器
/// </summary>
public class TimerManager : Singleton<TimerManager>
{
    private LinkedList<Timer> m_TimerList = new LinkedList<Timer>();

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Initialize()
    {
    }

    protected LinkedListNode<Timer> m_curTimer = null;

    /// <summary>
    /// 更新定时器
    /// </summary>
    public void Update(float deltaTime, float unscaledDeltaTime)
    {
        m_curTimer = m_TimerList.First;
        while (null != m_curTimer)
        {
            var cur_timer = m_curTimer;
            m_curTimer = m_curTimer.Next;

            try
            {
                if (!cur_timer.Value.Run(deltaTime, unscaledDeltaTime))
                {
                    if (m_TimerList == cur_timer.List)
                    {
                        m_TimerList.Remove(cur_timer);
                        cur_timer.Value.Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);

                // 回调对象被删除
                if (m_TimerList == cur_timer.List)
                {
                    m_TimerList.Remove(cur_timer);
                    cur_timer.Value.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// 增加定时器
    /// <param name="unScale">True:不受Time.scale影响,false相反</param>
    /// </summary>
    public Timer AddOnceTimer(float duration, bool unScale, CallBack<object[]> handler, params object[] args)
    {
        return Internal_AddTimer(1, duration, unScale,handler, args);
    }

    /// <summary>
    /// 增加计数定时器
    /// <param name="unScale">True:不受Time.scale影响,false相反</param>
    /// </summary>
    public Timer AddCountTimer(float duration, bool unScale, CallBack<object[]> handler, uint count, params object[] args)
    {
        return Internal_AddTimer((int)count, duration, unScale, handler, args);
    }

    /// <summary>
    /// 增加持续定时器
    /// <param name="unScale">True:不受Time.scale影响,false相反</param>
    /// </summary>
    public Timer AddRepeatTimer(float duration, bool unScale, CallBack<object[]> handler, params object[] args)
    {
        return Internal_AddTimer(-1, duration, unScale, handler);
    }

    /// <summary>
    /// 重置指定定时器
    /// </summary>
    /// <param name="key"></param>
    public void ResetTimer(Timer timer)
    {
        if (timer == null)
            return;
        timer.Reset();
    }

    public void RemoveTimer(Timer timer)
    {
        if (timer == null)
            return;
        m_TimerList.Remove(timer);
        timer.Dispose();
        timer = null;
    }

    public void ClearTimer()
    {
        Debug.Log("总共计时器个数为：" + m_TimerList.Count);
        m_curTimer = m_TimerList.First;
        var cur_timer = m_curTimer;
        while (m_curTimer != null)
        {
            cur_timer = m_curTimer;
            m_curTimer = m_curTimer.Next;
            if (m_TimerList == cur_timer.List)
            {
                Debug.Log("从TimerManager中移除计时器=====");
                m_TimerList.Remove(cur_timer);
                cur_timer.Value.Dispose();
            }
        }
        Debug.Log("移除后总共计时器个数为：" + m_TimerList.Count);
    }

    /// <summary>
    /// 从对象池创建时间对象
    /// </summary>
    private Timer CreateObj()
    {
        return new Timer();
    }

    /// <summary>
    /// 增加定时器
    /// <param name="unScale">是否不被Time.scale影响</param>
    /// </summary>
    private Timer Internal_AddTimer(int count, float duration, bool unScale, CallBack<object[]> handler, params object[] args)
    {
        if (duration < 0.0f)
            return null;

        Timer timer = CreateObj();
        if (timer == null)
            return null;

        timer.Initialize(count, duration,unScale, handler, args);
        m_TimerList.AddFirst(timer);
        return timer;
    }

    /// <summary>
    /// 是否在运行
    /// </summary>
    public bool IsRunning(Timer timer)
    {
        var timerNode = m_TimerList.First;
        while (null != timerNode)
        {
            var curTimerNode = timerNode;
            timerNode = timerNode.Next;

            if (curTimerNode.Value == timer)
            {
                return true;
            }
        }
        return false;
    }
}
