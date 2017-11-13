using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public static class ExtendFunc
{

    public static GameObject FindExt(this GameObject obj, string path)
    {
        Transform go = obj.transform.Find(path);
        if (null == go)
        {
            return null;
        }
        else
        {
            return go.gameObject;
        }
    }

    public static void SetParentExt(this GameObject obj, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

    }
    public static Vector3 SetX(this Vector3 v, float value)
    {
        return new Vector3(value, v.y, v.z);
    }


    public static T GetChildComponet<T>(this GameObject obj, string path)
    {
        return obj.FindExt(path).GetComponent<T>();
    }

    public static List<T> ToList<T>(this T[] array)
    {
        List<T> list = new List<T>(array);
        return list;
    }


    public static List<T> Clone<T>(this List<T> array)
    {
        List<T> list = new List<T>();
        for (int n = 0; n < array.Count; n++)
        {
            list.Add(array[n]);
        }
        return list;
    }

    public static string ToSymble(this int num)
    {
        return string.Format(num > 0 ? "<color=red>+{0}</color>" : "<color=green>{0}</color>", num);
    }

    public static string ToSymble(this int num, string color1, string color2)
    {
        return string.Format(num >= 0 ? "<color=" + color1 + ">+{0}</color>" : "<color=" + color2 + ">{0}</color>", num);
    }

    public static int Clamp(this int num, int min, int max)
    {
        if (num < min) return min;
        return num > max ? max : num;
    }

    public static string Color(this string str, ConsoleColor color = ConsoleColor.Cyan)
    {
        return string.Format("<color={0}>{1}</color>", color.ToString().ToLower(), str);
    }

    public static string Color(this string str, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, str);
    }

    public static string Color(this string str, int color)
    {
        return string.Format("<color=#{0}>{1}</color>", color, str);
    }

    public static string GetVersion(this string str)
    {
        string[] strArr = str.Split('.');
        return strArr.Length < 3 ? null : strArr[2];
    }

    public static Dictionary<K, V> AddSingleton<K, V>(this Dictionary<K, V> dict, K key, V value)
    {
        V _;
        if (!dict.TryGetValue(key,out _))
        {
            dict.Add(key, value);
        }
        return dict;
    }


	#region random

	public static T Random<T>(this List<T> _list)
	{
        Assert.IsFalse(null == _list || _list.Count <= 0);
		return _list[UnityEngine.Random.Range(0, _list.Count)];
	}

	public static T Random<T>(this T[] _array)
	{
		Assert.IsFalse(null == _array || _array.Length <= 0);
		return _array[UnityEngine.Random.Range(0, _array.Length)];
	}

	#endregion



	#region byte

	public static bool WriteBytes(this byte[] _value, byte[] _string, int _index)
	{
		if (null == _value || null == _string || 0 > _index)
		{
			return false;
		}
		if (_value.Length - _index < _string.Length)
		{
			return false;
		}
		for (int i = 0, j = _index; i < _string.Length && j < _value.Length; i++, ++j)
		{
			_value[j] = _string[i];
		}
		return true;
	}

	public static byte[] ToBytes(this int _value) { return BitConverter.GetBytes(_value); }

    public static byte[] ToBytes(this ushort _value) { return BitConverter.GetBytes(_value); }

    public static byte[] ToBytes(this uint _value) { return BitConverter.GetBytes(_value); }

	public static uint ReadUint(this byte[] _value, int _index) { return BitConverter.ToUInt32(_value, _index); }
    public static ushort ReadUshort(this byte[] _value, int _index) { return BitConverter.ToUInt16(_value, _index); }

	public static byte[] ReverseHead(this byte[] _value)
	{
		Assert.IsTrue(_value.Length == 4 || _value.Length == 2, "this function only to reverse message head!");
		Array.Reverse(_value);
		return _value;
	}

	#endregion

	#region ComponentEx

	public static T GetComponentInChildren<T>(this GameObject _go, string _target) where T : Component
	{
		var child = _go.transform.Find(_target);
		Assert.IsNull(child, $"cannot find target -> {_target}");
        if (child != null)
            return child.GetComponent<T>();
        else
            return null;
	}


	#endregion

	#region Collection

	public static void AddSingleton<T>(this List<T> list, T singleton)
	{
		if (!list.Contains(singleton))
		{
			list.Add(singleton);
		}
	}

	public static void RemoveSafety<T>(this List<T> list, T singleton)
	{
		if (list.Contains(singleton))
		{
			list.Remove(singleton);
		}
	}

	public static void AddRangeiOS<T>(this List<T> _list, List<T> _addList)
	{
		for (int n = 0; n < _addList.Count; ++n)
		{
			_list.Add(_addList[n]);
		}
	}

	public static void RemoveSafety<K, V>(this Dictionary<K, V> dict, K key)
	{
		if (dict.ContainsKey(key))
		{
			dict.Remove(key);
		}
	}
	#endregion

}
