using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

public static class GameObjectTools
{
    private static Dictionary<string, string> path_caches = new Dictionary<string, string>();

    public static T CreateComponent<T>() where T: MonoBehaviour
    {
        GameObject obj2 = new GameObject();
        return obj2.AddComponent<T>();
    }

    public static Component CreateComponent(System.Type type)
    {
        GameObject obj2 = new GameObject();
        return obj2.AddComponent(type);
    }

    public static GameObject CreateGameObject(System.Type type)
    {
        GameObject obj2 = new GameObject();
        Component component = obj2.AddComponent(type);
        return obj2;
    }

    /// <summary>
    /// 拷贝组件
    /// </summary>
    public static Component CopyComponent(Component original, GameObject destination, bool isCreateNew = true)
    {
        System.Type type = original.GetType();

        Component copy = null;
        if (isCreateNew)
        {
            copy = destination.AddComponent(type);
        }
        else
        {
            copy = destination.GetComponent(type);
        }

        System.Reflection.FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(copy, fields[i].GetValue(original));
        }

        return copy;
    }

    /// <summary>
    /// 拷贝组件泛型版本
    /// </summary>
    public static T CopyComponent<T>(T original, GameObject destination, bool isCreateNew = true) where T : Component
    {
        System.Type type = original.GetType();

        Component copy = null;
        if (isCreateNew)
        {
            copy = destination.AddComponent(type);
        }
        else
        {
            copy = destination.GetComponent(type);
        }

        //  拷贝字段
        System.Reflection.FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(copy, fields[i].GetValue(original));
        }

        return copy as T;
    }

    public static void DestroyComponent(GameObject go, System.Type type)
    {
        Component component = go.GetComponent(type);
        if (component != null)
        {
            UnityEngine.Object.Destroy(component);
        }
    }

    public static void DestroyEmptyAnimation(GameObject go)
    {
        Animation component = go.GetComponent<Animation>();
        if ((component != null) && (component.GetClipCount() == 0))
        {
            UnityEngine.Object.Destroy(component);
        }
    }

    public static void dumpResource(GameObject go)
    {
        List<string> list = new List<string> {
            "dumo of '" + go.name + "'"
        };
        Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
        list.Add("Bones: " + componentsInChildren.Length);
        foreach (Transform transform in componentsInChildren)
        {
            list.Add("   " + transform.name);
        }
        SkinnedMeshRenderer componentInChildren = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (componentInChildren != null)
        {
            Mesh sharedMesh = componentInChildren.sharedMesh;
            list.Add("vertexCount: " + sharedMesh.vertexCount);
            list.Add("triangles: " + (sharedMesh.triangles.Length / 3));
            BoneWeight[] boneWeights = sharedMesh.boneWeights;
            list.Add("bws: " + boneWeights.Length);
            foreach (BoneWeight weight in boneWeights)
            {
                object[] args = new object[] { weight.boneIndex0, weight.weight0, weight.boneIndex1, weight.weight1, weight.boneIndex2, weight.weight2, weight.boneIndex3, weight.weight3 };
                list.Add(string.Format("  {0}A={1}, {2}B={3}, {4}C={5}, {6}D={7}", args));
            }
            if (list.Count > 200)
            {
                Debug.Log(string.Join("\n", list.ToArray()));
                list.Clear();
            }
        }
        Debug.Log(string.Join("\n", list.ToArray()));
    }

    public static GameObject FindChildByName(GameObject go, string name)
    {
        string str = null;
        if (path_caches.TryGetValue(name, out str))
        {
            Transform transform = go.transform.Find(str);
            if (transform != null)
            {
                return transform.gameObject;
            }
        }
        Transform[] transformArray = new Transform[0x400];
        int num = 0;
        transformArray[num++] = go.transform;
        while (num > 0)
        {
            Transform child = transformArray[--num];
            if (child.name == name)
            {
                path_caches[name] = GetPath(go.transform, child);
                return child.gameObject;
            }
            IEnumerator enumerator = child.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    transformArray[num++] = current;
                }
                continue;
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
        return null;
    }

    public static string GetPath(Transform parent, Transform child)
    {
        List<string> list = new List<string>();
        while ((child != null) && (child != parent))
        {
            list.Add(child.name);
            child = child.parent;
        }
        list.Reverse();
        return string.Join("/", list.ToArray());
    }

	public static bool SetLayers(GameObject parentObj, int layer)
	{
		if (parentObj == null)
			return false;

		parentObj.layer = layer;
        for (int n = parentObj.transform.childCount - 1; 0 <= n; n--)
        {
            if (n < parentObj.transform.childCount)
            {
                SetLayers(parentObj.transform.GetChild(n).gameObject, layer);
            }
        }

        parentObj.gameObject.layer = layer;

		return true;
	}

    public static void SetLayers(GameObject go, int layer, int mask)
    {
        GameObject[] objArray = new GameObject[1024];
        int num = 0;
        objArray[num++] = go;
        while (num > 0)
        {
            go = objArray[--num];
            if (((((int) 1) << go.layer) & mask) != 0)
            {
                go.layer = layer;
                IEnumerator enumerator = go.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        objArray[num++] = current.gameObject;
                    }
                    continue;
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
        }
    }

    public static void SetParent(GameObject go, GameObject parent, bool resetTransform)
    {
        if ((go != null) && (parent != null))
        {
            Transform transform = go.transform;
            if (resetTransform)
            {
                transform.parent = parent.transform;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                Vector3 localPosition = transform.localPosition;
                Quaternion localRotation = transform.localRotation;
                Vector3 localScale = transform.localScale;
                transform.parent = parent.transform;
                transform.localPosition = localPosition;
                transform.localRotation = localRotation;
                transform.localScale = localScale;
            }
        }
    }

    /// <summary>
    /// 递归激活
    /// </summary>
    public static void SetActiveRecursively(GameObject target, bool bActive)
    {
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 ||UNITY_5)
        for (int n = target.transform.childCount - 1; 0 <= n; n--)
            if (n < target.transform.childCount)
                SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
        target.SetActive(bActive);
#else
        target.SetActiveRecursively(bActive);
        #endif
        // 		SetActiveRecursivelyEffect(target, bActive);
    }

    /// <summary>
    /// 是否激活
    /// </summary>
    public static bool IsActive(GameObject target)
    {
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0)
        return (target.activeInHierarchy && target.activeSelf);
        #else
		return target.active;
        #endif
    }

    /// <summary>
    /// 重置Transform
    /// </summary>
    public static void ResetTransform(GameObject obj)
    {
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }
}

