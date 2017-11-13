/*
    Event Manager
* 
*/

using System;
using UnityEngine;
using System.Collections.Generic;

public class EventDispatcher
{
    public Dictionary<ValueType, Delegate> eventTable;

    public EventDispatcher()
    {

    }

    // Add event listener
    public void addEventListener(ValueType eventType, CallBack method)
    {
        if (!recordEvent(eventType, method)) return;
        eventTable[eventType] = Delegate.Combine((CallBack)eventTable[eventType], method);
    }

    public void addEventListener<T>(ValueType eventType, CallBack<T> method)
    {
        if (!recordEvent(eventType, method)) return;
        eventTable[eventType] = Delegate.Combine((CallBack<T>)eventTable[eventType], method);
    }

    public void addEventListener<T, U>(ValueType eventType, CallBack<T, U> method)
    {
        if (!recordEvent(eventType, method)) return;
        eventTable[eventType] = Delegate.Combine((CallBack<T, U>)eventTable[eventType], method);
    }

    public void addEventListener<T, U, V>(ValueType eventType, CallBack<T, U, V> method)
    {
        if (!recordEvent(eventType, method)) return;
        eventTable[eventType] = Delegate.Combine((CallBack<T, U, V>)eventTable[eventType], method);
    }

    // Remove event listener
    public void removeEventListener(ValueType eventType, CallBack method)
    {
        if (!removeEvent(eventType, method)) return;
        eventTable[eventType] = (CallBack)Delegate.Remove((CallBack)eventTable[eventType], method);
        removeType(eventType);
    }

    public void removeEventListener<T>(ValueType eventType, CallBack<T> method)
    {
        if (!removeEvent(eventType, method)) return;
        eventTable[eventType] = (CallBack<T>)Delegate.Remove((CallBack<T>)eventTable[eventType], method);
        removeType(eventType);
    }

    public void removeEventListener<T, U>(ValueType eventType, CallBack<T, U> method)
    {
        if (!removeEvent(eventType, method)) return;
        eventTable[eventType] = (CallBack<T, U>)Delegate.Remove((CallBack<T, U>)eventTable[eventType], method);
        removeType(eventType);
    }

    public void removeEventListener<T, U, V>(ValueType eventType, CallBack<T, U, V> method)
    {
        if (!removeEvent(eventType, method)) return;
        eventTable[eventType] = (CallBack<T, U, V>)Delegate.Remove((CallBack<T, U, V>)eventTable[eventType], method);
        removeType(eventType);
    }

    // Dispatch an event
    public void dispatchEvent(ValueType eventType)
    {
        if (eventTable == null)
            return;
        if (!eventTable.ContainsKey(eventType)) return;
        Delegate method;
        if (eventTable.TryGetValue(eventType, out method))
        {
            CallBack CallBack = method as CallBack;
            if (CallBack != null)
            {
                CallBack();
            }
        }
    }


    public void dispatchEvent<T>(ValueType eventType, T arg)
    {
        if (eventTable == null)
            return;
        if (!eventTable.ContainsKey(eventType)) return;
        Delegate method;
        if (eventTable.TryGetValue(eventType, out method))
        {
            CallBack<T> CallBack = method as CallBack<T>;
            if (CallBack != null)
            {
                CallBack(arg);
            }
        }
    }

    public void dispatchEvent<T, U>(ValueType eventType, T arg1, U arg2)
    {
        if (eventTable == null)
            return;
        if (!eventTable.ContainsKey(eventType)) return;
        Delegate method;
        if (eventTable.TryGetValue(eventType, out method))
        {
            CallBack<T, U> CallBack = method as CallBack<T, U>;
            if (CallBack != null)
            {
                CallBack(arg1, arg2);
            }
        }
    }

    public void dispatchEvent<T, U, V>(ValueType eventType, T arg1, U arg2, V arg3)
    {
        if (eventTable == null)
            return;
        if (!eventTable.ContainsKey(eventType)) return;
        Delegate method;
        if (eventTable.TryGetValue(eventType, out method))
        {
            CallBack<T, U, V> CallBack = method as CallBack<T, U, V>;
            if (CallBack != null)
            {
                CallBack(arg1, arg2, arg3);
            }
        }
    }

    // record event, if it doesn't already exists
    private bool recordEvent(ValueType eventType, Delegate method)
    {
        if (eventTable == null)
            eventTable = new Dictionary<ValueType, Delegate>();
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
        Delegate d = eventTable[eventType];
        if (d != null)
        {
            if (d.GetType() != method.GetType())
            {
                return false;
            }
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], method);
        }
        return true;
    }

    private bool removeEvent(ValueType eventType, Delegate method)
    {
        if (eventTable == null)
            return false;
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != method.GetType())
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private void removeType(ValueType eventType)
    {
        if (this.eventTable.ContainsKey(eventType) && (this.eventTable[eventType] == null))
        {
            this.eventTable.Remove(eventType);
        }
    }
}