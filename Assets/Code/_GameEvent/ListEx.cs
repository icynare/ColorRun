using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class ListEx<T> : List<T>, System.ICloneable
{
    public ListEx()
    {

    }

    public ListEx(int capacity) : base(capacity)
    {
    }

	public ListEx(IEnumerable<T> collection) : base(collection)
	{
	}

    public object Clone()
    {
        ListEx<T> cloneLst = new ListEx<T>(this.Capacity);

        for (int i = 0; i < this.Count; i++)
        {
            T cloneMember = (T)((ICloneable)this[i]).Clone();
            cloneLst.Add(cloneMember);
        }

        return cloneLst;
    }
}