/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/24 17:19:09
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf;
using System.Net.Sockets;
using System.Net;

public class NetModel : Singleton<NetModel> {



	public readonly Dictionary<ushort, Delegate> CodeToHandlerDic = new Dictionary<ushort, Delegate>();

    public readonly Dictionary<ushort, Type> CodeToTypeDic = new Dictionary<ushort, Type>();

	public void AddSocketListener<T>(ushort code, CallBack<T> callback)
	{
		if (CodeToHandlerDic.ContainsKey(code))
		{
			CodeToHandlerDic[code] = Delegate.Combine((CallBack<T>)CodeToHandlerDic[code], callback);
		}
		else
		{
			CodeToHandlerDic.Add(code, callback);
            if( !CodeToTypeDic.ContainsKey(code) )
            {
				var pars = callback.Method.GetParameters();
                CodeToTypeDic[code] = pars[0].ParameterType;
			}
		}
	}

	public void RemoveSocketListener<T>(ushort code, CallBack<T> callback)
	{
		if (CodeToHandlerDic.ContainsKey(code))
		{
			CodeToHandlerDic[code] = Delegate.Remove((CallBack<T>)CodeToHandlerDic[code], callback);
		}
	}

    private object heartbeatLock = new object();
    private int heartBeatTemp = 0;
    public void PushHeartbeat()
    {
        lock(heartbeatLock)
        {
            heartBeatTemp += 1;
        }
    }
    public bool PopHeartbeart()
    {
        lock(heartbeatLock)
        {
            if (heartBeatTemp > 0)
            {
				heartBeatTemp = 0;
                return true;
			}
        }
        return false;
    }


    public static bool TryGetIPFromHostOrAddress(string hostOrAddress,out IPAddress ipAddress)
    {
        hostOrAddress = hostOrAddress.Replace("http://", "").Replace("https://", "");
        try{
			IPAddress[] ips = Dns.GetHostAddresses(hostOrAddress);
			if (ips.Length > 0)
			{
				ipAddress = ips[0];
				return true;
			}
        }
        catch(Exception e)
        {
            //Debug.Log("Get");
        }
		ipAddress = null;
        return false;

	}
}

public class MsgObject
{
	public ushort code;//协议号
	public IMessage obj;//协议类
	public MsgObject(ushort code, IMessage obj)
	{
		this.code = code;
		this.obj = obj;
	}
}
public enum SocketEvent
{
    Reconnected,
    Connected,
    SendHeartBeat,
    RcvHeartBeat,
    Closed,
    StartReconect,
    ReconnectFail,
    DnsGetHostFail,
}