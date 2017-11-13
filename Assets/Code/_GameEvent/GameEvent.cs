using UnityEngine;
using System.Collections;
using System;

// 游戏事件类
// 使用event的时候需要加上using修饰符，以便自动归还给Eventmanager
// 如果要保存Event的话，需要手动调用CloneEvent()方法
public class GameEvent : IDisposable, IPooledObjSupporter, ICloneable
{
    public EventName m_id = EventName.GECount;
    public QuickList<object> m_parameters = new QuickList<object>();

    public void AddParameter(object parameter)
    {
        m_parameters.Add(parameter);
    }

    public void Dispose()
    {
        EventManager.Instance.DestroyEvent(this);
    }

    public GameEvent CloneEvent()
    {
        return (GameEvent)Clone();
    }

    public object Clone()
    {
        GameEvent gameEvent = EventManager.Instance.CreateEvent();
        gameEvent.SetID(this.GetID());

        for (int n = 0; n < m_parameters.Count; ++n)
        {
			if (null == m_parameters[n])
				gameEvent.m_parameters.Add(m_parameters[n]);
			else if (m_parameters[n].GetType().IsValueType)
				gameEvent.m_parameters.Add(m_parameters[n]);
			else
				gameEvent.m_parameters.Add(((ICloneable)m_parameters[n]).Clone());
        }

        return gameEvent;
    }

    public EventName GetID()
    {
        return this.m_id;
    }

    public T GetParameter<T>(int index)
    {
        if (index >= m_parameters.Count || index < 0)
        {
			Debug.LogError("Error : The Event Parameter index out of range!");
            return default(T);
        }
        return ((T)m_parameters[index]);
    }

	public object Get(int index)
	{
		if (index >= m_parameters.Count || index < 0)
		{
			return null;
		}

		return m_parameters[index];
	}

    public void GetParameter<T>(ref T parameter, int index)
    {
        if (m_parameters == null || index >= m_parameters.Count)
        {
			Debug.LogError("Error: The Event Parameter index > mParameters.Count!");
            parameter = default(T);
        }
        else if (m_parameters[index] != null && m_parameters[index].GetType() != parameter.GetType())
        {
			Debug.LogError("Error: The Event Parameter Type Error!");
            parameter = default(T);
        }
        else
        {
            parameter = (T)m_parameters[index];
        }
    }

    public int GetParameterCount()
    {
        return m_parameters.Count;
    }

    public void Reset()
    {
        if (this.m_parameters != null)
            this.m_parameters.Clear();
        else
            this.m_parameters = new QuickList<object>(2);

        this.m_id = EventName.GECount;
    }

    public void SetID(EventName id)
    {
        m_id = id;
    }
}