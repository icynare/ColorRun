using UnityEngine;
using System.Collections.Generic;
using System.Text;
public class PingCheat : MonoBehaviour
{
	public List<int> pingList;
	private int maxNum = 1;
	private string str = "ms";
	private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.
	public Rect startRect = new Rect(10, 0, 60, 22); // The rect the window is initially displayed at.
	private long lastSendTime = long.MaxValue;
	private long lastRcvTime = long.MaxValue;
	private int avg = 0;


	public PingCheat()
	{
		pingList = new List<int>();
	}

	private void OnEnable()
	{
		NetManager.Instance.addEventListener(SocketEvent.SendHeartBeat, OnSendHeartBeat);
		NetManager.Instance.addEventListener(SocketEvent.RcvHeartBeat, OnRevHeartBeat);
	}

	private void OnDisable()
	{
		NetManager.Instance.removeEventListener(SocketEvent.SendHeartBeat, OnSendHeartBeat);
		NetManager.Instance.removeEventListener(SocketEvent.RcvHeartBeat, OnRevHeartBeat);
	}


	private void OnSendHeartBeat()
	{
        if( lastSendTime == long.MaxValue )
		    lastSendTime = ServerTimeManager.getClientUnixTimeMS();
		NetManager.Log("SendHeartBeat:" + lastSendTime / 1000);
		//Debug.LogFormat("send:{0}", ServerTimeManager.getClientUnixTimeMS());
	}

	private void OnRevHeartBeat()
	{
		NetManager.Log("RevHeartBeat:" + lastSendTime / 1000);
        //Debug.LogFormat("Send:{1} Rcv:{0}",ServerTimeManager.getClientUnixTimeMS(),lastSendTime);
		lastRcvTime = ServerTimeManager.getClientUnixTimeMS();
		addPing(lastRcvTime - lastSendTime);
        lastSendTime = long.MaxValue;
	}

	//pingTime:ms
	private void addPing(long pingTime)
	{
		//Debug.LogFormat("ADDPing:{0}",pingTime);
		int t = ((int)(pingTime));
		if (pingList.Count >= maxNum)
			pingList.RemoveAt(maxNum - 1);
		pingList.Insert(0, t);
		int sum = 0;
		for (int i = 0; i < pingList.Count; i++)
		{
			sum += pingList[i];
		}
		avg = (sum / pingList.Count);

	}

	void OnGUI()
	{
		// Copy the default label skin, change the color and the alignement
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
		GUI.color = Color.white;
		startRect.x = Screen.width - startRect.width;
		//        startRect.height = 20+18;
		//        GUI.Label(startRect, str, style);
		startRect = GUI.Window(this.GetHashCode(), startRect, DoMyWindow, "");
	}

	void DoMyWindow(int windowID)
	{
		var temp = ServerTimeManager.getClientUnixTimeMS() - lastSendTime;

		if (temp > avg)
        {
            if (temp < 10000)
                str = temp.ToString() + "ms";
            else
                str = "9999+ms";
		}
		else
			str = avg.ToString() + "ms";

		GUI.Label(new Rect(0, 0, startRect.width, startRect.height), str, style);
		//GUI.Label(new Rect(0, 0, startRect.width, startRect.height), conent, style);
		//        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}
}

