﻿﻿﻿/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/24 19:01:10
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using Google.Protobuf;

//管理socket和worker
public class NetManager : SingletonEventDispatcher<NetManager>
{

    public static bool logNetDetailMsg = false;

    public class ConnectResult
    {
        public bool successed = true;
        public bool callbacked = false;
        public SocketErrorType errorType;
        public string errorString;
    }
    public enum SocketErrorType
    {
        SocketException,
        Exception
    }

    public enum SocketConnectState
    {
		Disconnected,
        TryConnecting,
        Connecting,
    }
    private bool firstConnected = false;
    private bool isInited = false;
    private SocketConnectState socketState = SocketConnectState.Disconnected;
    const int MaxReConnectCount = 3;
    private int reconnectCount = 0;
    private string host;
    private int port;
    private bool autoReconnect = true;

    private Socket socket;
    private int timeout = 3000;
    private bool heartbeatNoReturn = false;
    private NetWorker worker;
    private AddressFamily curAddressFamily = AddressFamily.InterNetwork;

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.addEventListener<float>(EventName.GameLoop, Update);
        EventManager.Instance.addEventListener(EventName.ForceReconnect, Reconnect);
        //RuntimeData.debugTool.Add("检查重连", CheckReconnect);
        //RuntimeData.debugTool.Add("输出网络详细信息", ChangeLogEnable);
    }

    public override void UnInitialize()
    {
        EventManager.Instance.removeEventListener<float>(EventName.GameLoop, Update);
        EventManager.Instance.removeEventListener(EventName.ForceReconnect, Reconnect);
        this.Close();
        base.UnInitialize();
    }

    public void Connect(string hostOrAddress = null, int portArg = 0, int timeoutArg = 3000)
    {
		if (hostOrAddress != null)
			this.host = hostOrAddress;
		if (portArg != 0)
			this.port = portArg;
		if (timeoutArg != 3000)
			this.timeout = timeoutArg;

        if (socketState==SocketConnectState.TryConnecting || Vaild())
        {
            Debug.Log("Can't connect twice".Color(ConsoleColor.Red));
            return;
        }

		

        IPAddress ipAddress;
        if( NetModel.TryGetIPFromHostOrAddress(host,out ipAddress))
        {
			curAddressFamily = ipAddress.AddressFamily;
            Debug.LogFormat(">>HOST:{3} IP:{0} PORT:{1} TYPE:{2}".Color(ConsoleColor.Green),
                            ipAddress, port,curAddressFamily,host);
        }
        else
        {
            Debug.LogFormat(">>GetIP Failed,HOST:{0}".Color(ConsoleColor.Red),host);
            dispatchEvent(SocketEvent.DnsGetHostFail);
            return;
        }
        
        isInited = true;
        ConnectResult state = new ConnectResult();
        state.successed = true;

        socket = new Socket(curAddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        Debug.Log("connect to " + host + ":" + port);

        try
        {
            socket.BeginConnect(new IPEndPoint(ipAddress, port), new AsyncCallback(connectCB), state);
            Coroutine cor = new Coroutine(WaitForConnect(state));
            this.socketState = SocketConnectState.TryConnecting;
        }
        catch (SocketException e)
        {
            Debug.LogFormat("connectTo SocketException: {0}".Color(ConsoleColor.Red), e.ToString());
            state.errorType = SocketErrorType.SocketException;
            state.errorString = e.ToString();
            state.successed = false;
            state.callbacked = true;
            ProcessConnectStatus(state);
        }
        catch (Exception e)
        {
            Debug.LogFormat("connectTo error: {0}".Color(ConsoleColor.Red), e.ToString());
            state.errorType = SocketErrorType.Exception;
            state.errorString = e.ToString();
            state.successed = false;
            state.callbacked = true;
            ProcessConnectStatus(state);
        }
        InitHeartbeatState();
    }

    private void connectCB(IAsyncResult ar)
    {
        ConnectResult state = null;
        try
        {
            state = (ConnectResult)ar.AsyncState;
            socket.EndConnect(ar);
        }
        catch (SocketException e)
        {
            state.errorType = SocketErrorType.SocketException;
            state.errorString = e.ToString();
            state.successed = false;
        }
        catch (Exception e)
        {
            state.errorType = SocketErrorType.Exception;
            state.errorString = e.ToString();
            state.successed = false;
        }
        if (state != null)
        {
            state.callbacked = true;
        }
    }

    private IEnumerator WaitForConnect(ConnectResult state)
    {
        float connectTimeAcc = 0.0f;
        bool connectTimeout = false;
        while (!state.callbacked)
        {
            // 等待异步连接返回
            connectTimeAcc += Time.deltaTime;
            if (connectTimeAcc > timeout)
            {
                connectTimeout = true;
                break;
            }
            yield return null;
        }
        if (connectTimeout)
        {
            state.errorString = string.Format("Connect timeout:{0} second",connectTimeAcc);
            state.errorType = SocketErrorType.SocketException;
            state.successed = false;
            ProcessConnectStatus(state);
        }
        else
        {
            ProcessConnectStatus(state);
        }
    }

    private void ProcessConnectStatus(ConnectResult state)
    {
        if (state.successed)
        {
            //Debug.Log("Connect Successed".Color(ConsoleColor.Green));
            OnSocketConnected();
        }
        else
        {
            Debug.LogFormat("Connect Failed,ErrorType:{0},ErrorString:{1}".Color(ConsoleColor.Red), state.errorType, state.errorString);
            SocketBreakHandler();
        }
    }

    public void CheckReconnect()
    {
        if (!isInited)
            return;
        var b = Vaild();
        if (!b) //正在工作中
        {
            if (!autoReconnect || reconnectCount == 0)
            {
                Reconnect();
            }
            else
            {
                Debug.LogFormat(
                    "Socket is already reconnecting.AutoReconnect:{0} reconnectCount:{1}",
                    autoReconnect,autoReconnect);
            }
        }
        else
        {
            Debug.LogFormat("Not Need to Reconnect Because  !socking valid:{0} = False",b);
        }

    }

    private void Reconnect()
    {
        if(this.reconnectCount == 0)
        {
            dispatchEvent(SocketEvent.StartReconect);
        }
        Debug.Log("Try Reconnecting".Color(ConsoleColor.DarkYellow));
        this.CloseSocketAndWorker();
        this.reconnectCount++;
        if (reconnectCount < MaxReConnectCount)
        {
            this.Connect();
        }
        else
        {
            this.reconnectCount = 0;
            Debug.Log("Try Reconnecting Failed!".Color(ConsoleColor.Red));
            dispatchEvent(SocketEvent.ReconnectFail);
        }
    }

    private void OnSocketConnected()
    {
        InitHeartbeatState();
        socketState = SocketConnectState.Connecting;
        reconnectCount = 0;

        if (this.firstConnected && this.autoReconnect)
        {
            StartWork();
            Debug.Log("Try Reconnecting Successed".Color(ConsoleColor.Green));
            dispatchEvent(SocketEvent.Reconnected);
        }
        else
        {
            firstConnected = true;
            StartWork();
            Debug.Log("First Connected".Color(ConsoleColor.Green));
            dispatchEvent(SocketEvent.Connected);
        }

    }

    private void SocketBreakHandler()
    {
        socketState = SocketConnectState.Disconnected;
        if (this.autoReconnect)
        {
            //isConnecting = false;
			Reconnect();
		}
        else
        {
            CloseSocketAndWorker();
			dispatchEvent(SocketEvent.Closed);
		}

    }

    public bool IsConnecting()
    {
        return socketState == SocketConnectState.Connecting;
    }

    private void CloseSocketAndWorker()
    {
        if (this.socket != null)
        {
			Debug.Log("CloseSocketAndWorker");
			try
            {
                if (socket.Connected)
                    this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                this.socket = null;
            }
            catch (Exception ex)
            {
                Debug.LogFormat("Close Exception: {0}", ex.ToString());
            }
        }
        socket = null;
        this.socketState = SocketConnectState.Disconnected;
        ClearWorker();
    }

    public void Close()
    {
        this.firstConnected = false;
        this.CloseSocketAndWorker();
        dispatchEvent(SocketEvent.Closed);
    }

    public void Update(float deltaTime)
    {
        //LogFormat("Netmanager.Update,{0}",isConnecting);
        if (socketState == SocketConnectState.Connecting)
        {
            if (NetModel.Instance.PopHeartbeart())
                ReceiveHeartBeat();
            CheckHeartBeatReturn();
            if (!Vaild())
                SocketBreakHandler();
            else
                CheckWorkerRcv();
        }

    }
    

    public bool Vaild()
    {
        if (socketState == SocketConnectState.Disconnected || heartbeatNoReturn)
            return false;
        if (  (this.socket != null) && this.socket.Connected && worker != null && worker.SocketValid)
        {
            //Log("Vaild:True");
            return true;
        }
        //Log("Vaild:False");
        return false;
    }

    private void StartWork()
    {
        Log("Manager.StartWork");
        ClearWorker();
        //Log("ReceiveTimeout" + socket.ReceiveTimeout.ToString());
        socket.ReceiveTimeout = timeout;
        worker = new NetWorker();
        worker.Start(socket);

    }

    private void ClearWorker()
    {
        
        if (worker != null)
        {
            Log("CleanNetWorker");
			worker.Stop();
		}
        worker = null;
    }

    #region msg handler
    public void AddSocketListener<T>(ushort code, CallBack<T> callback)
    {
        NetModel.Instance.AddSocketListener<T>(code, callback);
    }

    public void RemoveSocketListener<T>(ushort code, CallBack<T> callback)
    {
        NetModel.Instance.RemoveSocketListener<T>(code, callback);
    }

    public void SendMsg(ushort msgCode, IMessage obj)
    {
        if( msgCode!=0 )
            Debug.LogFormat("NetManager start SendMSg code:{0} obj:{1}", msgCode, obj);
        if (Vaild())
        {
            worker.PushSendMsg(msgCode, obj);
        }
        else
        {
            Debug.LogFormat("Socket not valid,send failed,code:{0} obj:{1}",msgCode,obj);
        }
    }
    private void CheckWorkerRcv()
    {
        MsgObject obj = worker.PopRcvMsg();
        while (obj != null)
        {
            DispatchSocketData(obj);
            obj = worker.PopRcvMsg();
        }
    }

    private bool DispatchSocketData(MsgObject msgObj)
    {
        Delegate del;
        NetModel.Instance.CodeToHandlerDic.TryGetValue(msgObj.code, out del);
        if (del != null)
        {
            del.DynamicInvoke(msgObj.obj);
            return true;
        }
        else
        {
            Debug.Log($"receive unregister net message -> {msgObj.code}");
            return false;
        }

    }

    private CallBack<object[]> heartBeatCallback;
    private Timer timer;
    private int heartbeatWaitMaxTime = 3000;
    public void SetHeartBeat(CallBack<object[]> callback,uint time)
    {
        heartbeatWaitMaxTime = Math.Max(heartbeatWaitMaxTime, (int)(time) *1000 - 1000);
        heartBeatCallback = callback;
        if (timer == null)
        {
            timer = TimerManager.Instance.AddRepeatTimer(time, true, callhandler);
        }
    }

    private long lastHeartbeatStartWait = long.MaxValue;
    private void callhandler(object[] obj)
    {
        if (!Vaild())
        {
            return;
        }
        else if (heartBeatCallback != null)
        {
            heartBeatCallback(null);
            lastHeartbeatStartWait = DateTime.Now.Ticks/10000;
            dispatchEvent(SocketEvent.SendHeartBeat);
        }

    }

    private void ReceiveHeartBeat()
    {
        lastHeartbeatStartWait = long.MaxValue;
        dispatchEvent(SocketEvent.RcvHeartBeat);
    }
    private void CheckHeartBeatReturn()
    {
        var dif = DateTime.Now.Ticks / 10000 - lastHeartbeatStartWait;
        if( dif > heartbeatWaitMaxTime )
        {
            heartbeatNoReturn = true;
            Debug.Log("HeartBeat No Return!!!".Color(ConsoleColor.Red));
        }
    }
    private void InitHeartbeatState()
    {
		heartbeatNoReturn = false;
		lastHeartbeatStartWait = long.MaxValue;
    }
    #endregion


    #region log

    public static void Log(string msg)
    {
        if (logNetDetailMsg)
            Debug.Log(msg);
    }

    public static void LogFormat(string msg, params object[] args)
    {
        if (logNetDetailMsg)
            Debug.LogFormat(msg, args);
    }

	public static void LogError(string msg)
	{
		if (logNetDetailMsg)
			Debug.LogError(msg);
	}

	public static void LogErrorFormat(string msg, params object[] args)
	{
		if (logNetDetailMsg)
			Debug.LogErrorFormat(msg, args);
	}

    public static void ChangeLogEnable()
    {
        logNetDetailMsg = !logNetDetailMsg;
    }

    #endregion log

}

