using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientEnter : MonoBehaviour
{

    public bool useTargetFrameRate = true;
    public float DetlaTime = 0f;
    public float RealDetlaTime = 0f;
    public int targetFPS = 45;
    public bool hasDebug = true;

    //load asset in assetbundle if true,otherwise load from EditorResources
    public bool useAssetsBundleInEditor = false;


    private Client _client;

    private void Awake()
    {
        
        if (useTargetFrameRate)
        {
            Application.targetFrameRate = targetFPS;
        }

		ResourceManager.Instance.useAssetsBundleInEditor = useAssetsBundleInEditor;
        Application.runInBackground = true;

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//开启debug模式
#if (UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_DEV)
		Debug.logger.logEnabled = hasDebug;
#else
        Debug.logger.logEnabled = false;
#endif

        UpdateComplete();
		
		DontDestroyOnLoad(this);

        EventManager.Instance.dispatchEvent<bool>((int)EventName.ApplicationStart, true);

        
    }

    public void UpdateComplete()
	//private void UpdateComplete()
	{
		_client = Client.Instance;
		StartCoroutine(_client.Initialize(this));
	}

    private float _lastRealTime = 0f;
	void Update()
	{
		RealDetlaTime = Time.realtimeSinceStartup - _lastRealTime;
		_lastRealTime = Time.realtimeSinceStartup;
		DetlaTime = Time.deltaTime;
        if (_client != null && _client.inited)
			_client.MainLoop();

        if(Application.platform == RuntimePlatform.WindowsPlayer)
        {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			} 
        }
	}

	void LateUpdate()
	{
		if (_client != null && _client.inited)
			_client.LateUpdate();
	}


	void OnApplicationQuit()
	{
		if (_client != null && _client.inited)
			_client.UnInitialize();
	}

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Debug.LogFormat("^^^^^OnApplicationFocus:True".Color(System.ConsoleColor.Red));
            NetManager.Instance.CheckReconnect();
            Debug.Log(ServerTimeManager.ToRealTime(ServerTimeManager.getClientUnixTimeMS()));
            Debug.Log("返回时间戳" + ServerTimeManager.getClientUnixTimeMS());
            //if (ServerTimeManager.getClientUnixTimeMS() - RuntimeData.nowTime > 30000)
            //    EventManager.Instance.dispatchEvent(EventName.ReFocus);
        }
        else
        {
            Debug.LogFormat("^^^^^OnApplicationFocus:False".Color(System.ConsoleColor.Red));
            //RuntimeData.nowTime = ServerTimeManager.getClientUnixTimeMS();
            //Debug.Log(ServerTimeManager.ToRealTime(RuntimeData.nowTime));
            //Debug.Log("退出时间戳" + RuntimeData.nowTime);
        }
        //Debug.LogFormat("^^^^^OnApplicationFocus:{0}".Color(System.ConsoleColor.Red), focus);
    }

    private void OnApplicationPause(bool pause)
    {
        //Debug.LogFormat("^^^^^^OnApplicationPause:{0}".Color(System.ConsoleColor.Red), pause);
        //NetManager.Instance.CheckReconnect();
	}

	void OnDestroy()
	{

	}
}
