/*********************************************************************
* Autor：zengruihong 
* Mail：zrh@talkmoney.cn
* CreateTime：2017/08/24 13:02:11
* Description：

*********************************************************************
* Copyright ©2017 Guangzhou Luwei Tech Co.,Ltd.ALL Rights Reserved
*********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Google.Protobuf;
using System.Threading;
using System;



//负责消息发送和接收，使用但是不会管理socket
public class NetWorker{
	
	const int RCV_BUF_LEN = 4096;

	//线程等待时间
	const int SEND_INTERVAL = 30;
	const int RCV_INTERVAL = 30;
	/// <summary>
	/// 发送数据队列同步锁。
	/// </summary>
	private readonly object sendQueueLocker = new object();
	/// <summary>
	/// 网络通信同步锁。
	/// </summary>
	private readonly object rcvQueueLocker = new object();

	private Thread sendThread;
	private Thread rcvThread;
	//发送开关
	private bool sendSwitch = true;
    private bool rcvSwitch = true;

	private Queue<MsgObject> sendQueue;
	private Queue<MsgObject> rcvQueue;

	private Socket socket;
    public bool SocketValid = true;

	private byte[] rcvBuf;
	private byte[] rcvCacheBuf;
    private byte[] codeBytes;
    private byte[] lenBytes;

	public NetWorker()
	{
		
	}

    public void Start(Socket socket )
	{
		this.socket = socket;
        sendQueue = new Queue<MsgObject>();
        rcvQueue = new Queue<MsgObject>();

		SocketValid = true;
		StartSendThread();
        rcvCacheBuf = new byte[0];
        rcvBuf = new byte[RCV_BUF_LEN];
		codeBytes = new byte[ProtoBufTool.CODE_SIZE];
        lenBytes = new byte[ProtoBufTool.LENGTH_SIZE];
        StartRcvThread();
	}
	
	private void StartSendThread()
	{
		sendThread = new Thread (new ThreadStart (AsynSend));
		sendThread.IsBackground = true;
		if (!sendThread.IsAlive) 
		{
			NetManager.Log ("Start AsynSend:" + this.sendSwitch);
			sendThread.Start ();
		}
	}

	private void AsynSend()
	{
		while (sendSwitch && SocketValid) 
		{
			this.DoSend ();
			Thread.Sleep (SEND_INTERVAL);
		}
        Debug.Log("Stop AsynSend:" + this.sendSwitch);
	}

	private void DoSend()
	{
		//TODO:Combine Msgs
		if ((this.socket == null) || (this.socket.Connected == false))
		{
            SocketValid = false;
            //NetManager.LogFormat("DoSend But socket isn't valid!");
			return;
		}
        MsgObject msg = PopSendMsg();
		while (msg != null) 
		{
			try
			{
				int length = 0;
                byte[] data = ProtoBufTool.ToByteArray(msg.code, msg.obj, ref length);
 
                int ret = socket.Send(data, 0, length + ProtoBufTool.HEAD_SIZE, SocketFlags.None);
                int totalLen = length + ProtoBufTool.HEAD_SIZE;
                if( ret != totalLen )
                {
                    Debug.LogErrorFormat("Socket send data, len={0} while send_len={1}", totalLen, ret);
                }
                else if (msg.code != 0)
                {
					Debug.LogFormat("Send message code:{0},{1}".Color(ConsoleColor.Green), msg.code, msg.obj);
                }
			}
            catch (SocketException e)
			{
                Debug.LogErrorFormat("Send Msg SocketException Code:{0} Msg:{1}\n{2}",msg.code,msg.obj,e);
				SocketValid = false;
                break;
			}
            catch( Exception e )
            {
                Debug.LogErrorFormat("Send Msg Exception Code:{0} Msg:{1}\n{2}", msg.code, msg.obj, e);
            }
            msg = PopSendMsg();
		}
	}


	private MsgObject PopSendMsg()
	{
		MsgObject msg = null;
		lock (sendQueueLocker) 
		{
			if (sendQueue.Count>0) 
			{
				msg = sendQueue.Dequeue();
			}
		}
		return msg;
	}

	private void StartRcvThread()
	{
        NetManager.Log("Start AsynRcv");
        rcvThread = new Thread(new ThreadStart(AsynRcv));
		rcvThread.IsBackground = true;
        if( !rcvThread.IsAlive)
		    rcvThread.Start();

	}

    int timeoutCount = 0;
	private void AsynRcv()
	{
        timeoutCount = 0;
		while (rcvSwitch && SocketValid)
		{
			this.DoReceive();
            Thread.Sleep(RCV_INTERVAL);
		}
        Debug.Log("Stop AsynRcv:" + this.sendSwitch);
	}

    private void DoReceive()
    {
        if ( socket == null || !socket.Connected )
		{
			//与服务器断开连接跳出循环
            Debug.Log("DoReceive:Socket isn't vaild");
			SocketValid = false;
            return;
		}
		try
		{
			//Receive方法中会一直等待服务端回发消息
			//如果没有回发会一直在这里等着。
            if (socket.Available > 0)
			{
                int revLen = socket.Receive(rcvBuf);

				if (revLen > 0)
				{
					byte[] bytes = new byte[revLen + rcvCacheBuf.Length];
					//把接收的数据和前面的数据合并
                    Array.Copy(rcvCacheBuf, bytes, rcvCacheBuf.Length);
                    Array.Copy(rcvBuf, 0, bytes, rcvCacheBuf.Length, revLen);
					//Debuger.Log(ByteArrayHelper.ByteToString(bytes));
					SplitPackage(bytes, 0);
				}
			}


		}
        catch( SocketException e )
        {
            if( e.SocketErrorCode == SocketError.TimedOut && timeoutCount < 1 )
            {
				Debug.LogFormat("Socket timeoutCount {0}", timeoutCount);
				timeoutCount++;
            }
            else
            {
                Debug.LogFormat("Do Receivie:SocketException,{0}", e);
                SocketValid = false;
            }
        }
		catch (Exception e)
		{
            Debug.Log("Do Receivie:Exception,{0}" + e);
			SocketValid = false;
		}
    }

    private void SplitPackage(byte[] bytes, uint index)
    {
        
		//在这里进行拆包，因为一次返回的数据包的数量是不定的
		//所以需要给数据包进行查分。
		while (true)
		{
			if (bytes.Length - index >= ProtoBufTool.HEAD_SIZE)
			{
				Array.Copy(bytes, (int)index + ProtoBufTool.CODE_BEGIN, codeBytes, 0, ProtoBufTool.CODE_SIZE);
				Array.Copy(bytes, (int)index + ProtoBufTool.LENGTH_BEGIN, lenBytes, 0, ProtoBufTool.LENGTH_SIZE);

				Array.Reverse(codeBytes, 0, ProtoBufTool.CODE_SIZE);
				Array.Reverse(lenBytes, 0, ProtoBufTool.LENGTH_SIZE);


				ushort msgCode = codeBytes.ReadUshort(0);
				uint msgLength = lenBytes.ReadUint(0);

				int totalLength = (int)msgLength + ProtoBufTool.HEAD_SIZE;

				if (bytes.Length - index >= totalLength)
				{
					index += ProtoBufTool.HEAD_SIZE;
                    IMessage obj = ConvertSocketData(msgCode, bytes, (int)index, (int)msgLength);
					index += (uint)msgLength;
					if (obj == null)
					{
						Debug.LogError("解析协议出错：" + msgCode);
					}
					else
					{
                        MsgObject msg = new MsgObject(msgCode,obj);
                        lock(rcvQueueLocker)
                        {
                            rcvQueue.Enqueue(msg);
                        }
						if (msgCode != 0)
						{
							Debug.LogFormat("Receive message code:{0} len:{1} #{2}".Color(ConsoleColor.Yellow), msg.code, totalLength, msg.obj);
						}
						//else
						//{
						//	dispatchEvent(LocalEvent.ReceiveHeadBeat);
						//}
					}
				}
				else
				{
					NetManager.Log("包的长度不够：" + (bytes.Length - (int)index) + "_" + totalLength);
                    rcvCacheBuf = new byte[bytes.Length - index];
					Array.Copy(bytes, index, rcvCacheBuf, 0, bytes.Length - index);
					break;
				}

			}
			else
			{
				rcvCacheBuf = new byte[bytes.Length - index];
				Array.Copy(bytes, index, rcvCacheBuf, 0, bytes.Length - index);
				break;
			}
		}
    }

    private IMessage ConvertSocketData(ushort code, byte[] bdata, int index, int length)
	{
        if (code == 0)
            NetModel.Instance.PushHeartbeat();
        Type type;
		if (NetModel.Instance.CodeToTypeDic.TryGetValue(code, out type))
		{
            IMessage obj = ProtoBufTool.Parse(bdata, type, index, length) as IMessage;
			return obj;
		}
		else
		{
			Debug.LogFormat("Receive unregister net message code:{0}",code);
		}
		return null;

	}


	//Push一条Msg，等待被发送，20ms一次
	public void PushSendMsg(ushort code, IMessage msg)
	{
		lock (sendQueueLocker)
		{
			sendQueue.Enqueue (new MsgObject(code,msg));
		}
	}

    public MsgObject PopRcvMsg()
	{
        lock( rcvQueueLocker)
        {
            if (rcvQueue.Count > 0)
                return rcvQueue.Dequeue();
        }
        return null;
	}


	public void Stop()
	{
        lock(sendQueueLocker)
        {
            this.sendQueue.Clear();
			this.sendQueue.TrimExcess();
        }
        lock (rcvQueueLocker)
        {
            this.rcvQueue.Clear();
            this.rcvQueue.TrimExcess();
        }
        this.sendSwitch = false;
        this.sendThread = null;
        this.rcvSwitch = false;
        this.rcvThread = null;
        Debug.Log("NetWorker Stoped");
        GC.Collect();
	}

}
