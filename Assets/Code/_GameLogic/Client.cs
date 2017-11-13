using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : Singleton<Client>
{
    public bool inited = false;
    private uint step = 0;
    private ClientEnter _clientEnter;
	public ClientEnter ClientEnter{ get {return _clientEnter;} }


	
    public IEnumerator Initialize(ClientEnter clientEnter)
    {
        _clientEnter = clientEnter;
        EventManager.Instance.Initialize();
        TimerManager.Instance.Initialize();
        XUIManager.Instance.Init();
        //NetManager.Instance.Initialize();
        
        EventManager.Instance.addEventListener(EventName.ReFocus, ResReFocus);

        //todo:use coroutine to loadConfig and wait


        LogicModules.Instance.Initialize();
		yield return null;

        //test code 
		//NetManager.Instance.addEventListener<string>(SocketEvent.Connected, connectHandler);
        //NetManager.Instance.addEventListener(SocketEvent.Reconnected, ReconnectedHandler);
        //NetManager.Instance.addEventListener(SocketEvent.StartReconect, MessageController.Instance.ShowLoading);
		//string ip = "119.23.155.182";
		//int port = 9008;
  //      NetManager.Instance.Connect(ip, port, 2000);
  //      NetManager.Instance.SetHeartBeat(SendHeartBeat, 5);
  //      yield return new WaitForSeconds(2);
        
        inited = true;

        EventManager.Instance.dispatchEvent(EventName.ShowBack);
        //XUIManager.Instance.ShowOrLoadView<BackGroundView>(XViewID.BackGroundView);


    }
    public void ReconnectedHandler()
    {
    }
    private void ResReFocus()
    {
        Debug.Log("切换进程已超过30s，进入断线重连");
    }
    private void SendHeartBeat(object[] obj)
    {
        //LoginController.Instance.ReqHeartBeat();
    }

	private void connectHandler(string str)
	{
        Debug.Log("connectSuccess");
	}

	public override void UnInitialize()
	{
		//UI destroy all
		EventManager.Instance.UnInitialize();
        //resource manager destroy
       
        LogicModules.Instance.UnInitialize();
        //NetManager.Instance.UnInitialize();
        ///NetManager.Instance.UnInitialize();
        //Debug.logger.logEnabled = false;
	}


    public void MainLoop()
    {
		float deltaTime = Time.deltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
        TimerManager.Instance.Update(deltaTime, unscaledDeltaTime);
		EventManager.Instance.dispatchEvent(EventName.GameLoop, deltaTime);
        //NetManager.Instance.Update();
        step++;
    }


	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		EventManager.Instance.dispatchEvent<float>(EventName.GameLoopLaterUpdate, deltaTime);
	}

    public ClientEnter ClientEnterIns
    {
        get{
            return _clientEnter;
        }
    }

}