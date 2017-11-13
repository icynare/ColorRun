using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager: Singleton<ResourceManager>
{

    Dictionary<string, Object> prefabPool = new Dictionary<string, Object>();
    Dictionary<string, Sprite> spritePool = new Dictionary<string, Sprite>();

    Dictionary<string, AudioClip> audioPool = new Dictionary<string, AudioClip>();

	Dictionary<string, Texture2D> texturePool = new Dictionary<string, Texture2D>();
    Dictionary<string, TextAsset> textPool = new Dictionary<string, TextAsset>();

    private AssetBundle _bundle = null;

    public bool useAssetsBundleInEditor = false;

    public Object LoadPrefab(string pathName)
    {
        Object obj;
        if (prefabPool.TryGetValue(pathName, out obj))
            return obj;

#if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE) && !UNITY_EDITOR
        return AssetBundleManager.Instance.LoadAsset<Object>(string.Format("prefab/{0}",pathName));
#else
		if(useAssetsBundleInEditor)
        {
            return AssetBundleManager.Instance.LoadAsset<Object>(string.Format("prefab/{0}",pathName));
        }
        else
        {
			string path = string.Format("Assets/EditorResources/Prefab/{0}.prefab", pathName);
			obj = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Object));
			prefabPool.Add(pathName, obj);
			return obj;
        }

#endif
    }

    //exp 
    public Sprite LoadSprite(string pathName,string packingTag = null)
    {
        Sprite obj;
        if (spritePool.TryGetValue(pathName, out obj))
            return obj;

#if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE) && !UNITY_EDITOR
        return AssetBundleManager.Instance.LoadSprite(string.Format("ui/{0}", pathName),packingTag);
#else
		if (useAssetsBundleInEditor)
        {
            return AssetBundleManager.Instance.LoadSprite(string.Format("ui/{0}", pathName),packingTag);
        }
        else
        {
			string path = string.Format("Assets/EditorResources/UI/{0}.png", pathName);
			Sprite tex = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
			spritePool.Add(pathName, tex);
			return tex;

        }
#endif
    }

	//exp to get:ui/gameui/emoji.png->emoji_1
	//pathName:gameui/emoji
	//slicedName：emoji_1
	public Sprite LoadSpriteInMulitple(string pathName, string slicedName)
	{
		Sprite obj;
        if (spritePool.TryGetValue(pathName+"/"+slicedName, out obj))
			return obj;

#if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE) && !UNITY_EDITOR
        return AssetBundleManager.Instance.LoadSpriteInMultiple(string.Format("ui/{0}", pathName),slicedName);
#else
		if (useAssetsBundleInEditor)
		{
            return AssetBundleManager.Instance.LoadSpriteInMultiple(string.Format("ui/{0}", pathName),slicedName);
		}
		else
		{
			string path = string.Format("Assets/EditorResources/UI/{0}.png", pathName);
            //Debug.Log(path);
            Object [] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path) as Object[];
            Sprite findingSprite = null;
            for (int i = 0; i < sprites.Length;i++)
            {
                spritePool.Add(pathName + "/" + sprites[i].name, sprites[i] as Sprite);
                if (sprites[i].name == slicedName)
                    findingSprite = sprites[i] as Sprite; 
            }
            return findingSprite;
		}
#endif
	}

    public AudioClip LoadClip(string pathName)
    {
        AudioClip clip;
        if (audioPool.TryGetValue(pathName, out clip))
            return clip;

#if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE) && !UNITY_EDITOR
        return AssetBundleManager.Instance.LoadAsset<AudioClip>(string.Format("audio/{0}", pathName));
#else
		if (useAssetsBundleInEditor)
        {
            return AssetBundleManager.Instance.LoadAsset<AudioClip>(string.Format("audio/{0}", pathName));
        }
        else
        {
            string path = string.Format("Assets/EditorResources/Audio/{0}.mp3", pathName);
            AudioClip src = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
            audioPool.Add(pathName, src);
            return src;
        }
#endif
    }

    public void SetNetTexture(RawImage image, string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        Texture2D tex;
        if (texturePool.TryGetValue(url, out tex))
        {
            image.texture = (Texture)tex;
            return;
        }

        Coroutine c = new Coroutine(LoadImg(image,url), true);
    }

    IEnumerator LoadImg(RawImage image, string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.isDone && string.IsNullOrEmpty(www.error) && (www.bytes.Length > 0))
        {
            Texture2D tex = new Texture2D(100, 100);
            tex.LoadImage(www.bytes, false);
            image.texture = (Texture)tex;

            if (!texturePool.ContainsKey(url))
                texturePool.Add(url, tex);
        }

    }
    
    public void LoadNetTexture(CallBack<Texture2D> callback, string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        Texture2D tex;
        if (texturePool.TryGetValue(url, out tex))
        {
            callback(tex);
            return;
        }

        Coroutine c = new Coroutine(LoadTexture(callback, url), true);
    }
    
    IEnumerator LoadTexture(CallBack<Texture2D> callback, string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error) && (www.bytes.Length > 0))
        {
            Texture2D tex = new Texture2D(100, 100);
            tex.LoadImage(www.bytes, false);
            

            if (!texturePool.ContainsKey(url))
                texturePool.Add(url, tex);
            callback(tex);
        }
        else
            callback(null);


    }

    public TextAsset LoadText(string pathName)
	{
        TextAsset text;
		if (textPool.TryGetValue(pathName, out text))
			return text;

#if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE) && !UNITY_EDITOR
        return AssetBundleManager.Instance.LoadAsset<TextAsset>(string.Format("text/{0}", pathName));
#else
		if (useAssetsBundleInEditor)
		{
			return AssetBundleManager.Instance.LoadAsset<TextAsset>(string.Format("text/{0}", pathName));
		}
		else
		{
			string path = string.Format("Assets/EditorResources/Text/{0}.txt", pathName);
			TextAsset src = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			textPool.Add(pathName, src);
			return src;
		}
#endif
	}

}
