using UnityEngine;
using System.Collections;
using System;

public class MonoBehaviourPool<T> : IObjectPool where T : class
{
	private System.Type destType;
	private Hashtable hashTableObjs;
	private Hashtable hashTableStatus;
	private ArrayList keyList;
	private string mName;
	private GameObject mParent;
	private int maxObjCount;
	private int minObjCount;
	private bool supportReset;
	private int shrinkPoint;

	public event CallBackObjPool MemoryUseOut;
	public event CallBackObjPool PoolShrinked;

	private object pool = new object();

	public MonoBehaviourPool()
	{
		this.hashTableObjs = new Hashtable();
		this.hashTableStatus = new Hashtable();
		this.keyList = new ArrayList();
		this.mName = string.Empty;
	}

	private bool CanShrink()
	{
		return (this.GetIdleObjCount() > this.shrinkPoint);
	}

	private int CreateOneObject()
	{
		object obj2 = null;
		try
		{
			GameObject obj3 = new GameObject(this.mName);
			if (this.mParent != null)
			{
				obj3.transform.parent = this.mParent.transform;
			}
			obj3.transform.localPosition = Vector3.zero;
			obj3.transform.localEulerAngles = Vector3.zero;
			obj3.transform.localScale = Vector3.one;
			obj3.transform.localRotation = Quaternion.identity;
			obj2 = obj3.AddComponent(typeof(T));
			obj3.SetActive(false);
		}
		catch
		{
			this.maxObjCount = this.CurrentObjCount;
			if (this.minObjCount > this.CurrentObjCount)
			{
				this.minObjCount = this.CurrentObjCount;
			}
			if (this.MemoryUseOut != null)
			{
				this.MemoryUseOut();
			}
			return -1;
		}
		int hashCode = obj2.GetHashCode();
		this.hashTableObjs.Add(hashCode, obj2);
		this.hashTableStatus.Add(hashCode, true);
		this.keyList.Add(hashCode);
		return hashCode;
	}

	public void Dispose()
	{
		System.Type type = typeof(IDisposable);
		if (type.IsAssignableFrom(this.destType))
		{
			IEnumerator enumerator = ((ArrayList)this.keyList.Clone()).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int current = (int)enumerator.Current;
					this.DistroyOneObject(current);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		this.hashTableStatus.Clear();
		this.hashTableObjs.Clear();
		this.keyList.Clear();
	}

	private void DistroyOneObject(int key)
	{
		object obj2 = this.hashTableObjs[key];
		IDisposable disposable = obj2 as IDisposable;
		if (disposable != null)
		{
			disposable.Dispose();
		}
		this.hashTableObjs.Remove(key);
		this.hashTableStatus.Remove(key);
		this.keyList.Remove(key);
		MonoBehaviour behaviour = obj2 as MonoBehaviour;
		if (behaviour != null)
		{
			UnityEngine.Object.Destroy(behaviour.gameObject);
		}
		else
		{
			Debug.LogError("DistroyOneObject error");
		}
	}

	public int GetIdleObjCount()
	{
		int num = 0;
		IEnumerator enumerator = this.keyList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				int current = (int)enumerator.Current;
				if ((bool)this.hashTableStatus[current])
				{
					num++;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return num;
	}

	public void GiveBackObject(int objHashCode)
	{
		if (this.hashTableStatus[objHashCode] != null)
		{
			lock (pool)
			{
				this.hashTableStatus[objHashCode] = true;
				if (this.supportReset)
				{
					((IPooledObjSupporter)this.hashTableObjs[objHashCode]).Reset();
				}
				object obj2 = this.hashTableObjs[objHashCode];
				if (obj2 != null)
				{
					MonoBehaviour behaviour = obj2 as MonoBehaviour;
					if (behaviour != null)
					{
						behaviour.gameObject.SetActive(false);
					}
				}
				if (this.CanShrink())
				{
					this.Shrink();
				}
			}
		}
	}

	public bool Initialize(System.Type objType, object[] cArgs, int minNum, int maxNum)
	{
		return this.Initialize(objType, cArgs, minNum, maxNum, null, "GameObj");
	}

	public bool Initialize(System.Type objType, object[] cArgs, int minNum, int maxNum, GameObject parent = null, string name = "GameObj")
	{
		if (minNum < 1)
		{
			minNum = 1;
		}
		if (maxNum < 2)
		{
			maxNum = 2;
		}
		this.destType = objType;
		this.minObjCount = minNum;
		this.maxObjCount = maxNum;
		double num = 1.0 - (((double)minNum) / ((double)maxNum));
		this.shrinkPoint = (int)(num * minNum);
		System.Type type = typeof(IPooledObjSupporter);
		if (type.IsAssignableFrom(objType))
		{
			this.supportReset = true;
		}
		this.mParent = parent;
		this.mName = name;
		return true;
	}

	private void InstanceObjects()
	{
		for (int i = 0; i < this.minObjCount; i++)
		{
			this.CreateOneObject();
		}
	}

	public object GetObject()
	{
		lock (pool)
		{
			object obj2 = null;
			IEnumerator enumerator = this.keyList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int current = (int)enumerator.Current;
					if ((bool)this.hashTableStatus[current])
					{
						this.hashTableStatus[current] = false;
						obj2 = this.hashTableObjs[current];
						break;
					}
				}

				if ((obj2 == null) && (this.keyList.Count < this.maxObjCount))
				{
					int num2 = this.CreateOneObject();
					if (num2 != -1)
					{
						this.hashTableStatus[num2] = false;
						obj2 = this.hashTableObjs[num2];
					}
				}
				if (obj2 != null)
				{
					MonoBehaviour behaviour = obj2 as MonoBehaviour;
					if (behaviour != null)
					{
						behaviour.gameObject.SetActive(true);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			return obj2;
		}
	}

	private void Shrink()
	{
		while (this.CurrentObjCount > this.minObjCount)
		{
			int key = -1;
			IEnumerator enumerator = this.keyList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int current = (int)enumerator.Current;
					if ((bool)this.hashTableStatus[current])
					{
						key = current;
						break;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				
			}

			if (key == -1)
			{
				break;
			}
			this.DistroyOneObject(key);
		}
		if (this.PoolShrinked != null)
		{
			this.PoolShrinked();
		}
	}

	public int CurrentObjCount
	{
		get
		{
			return this.keyList.Count;
		}
	}

	public int IdleObjCount
	{
		get
		{
			lock (pool)
			{
				return this.GetIdleObjCount();
			}
		}
	}

	public int MaxObjCount
	{
		get
		{
			return this.maxObjCount;
		}
	}

	public int MinObjCount
	{
		get
		{
			return this.minObjCount;
		}
	}
}
