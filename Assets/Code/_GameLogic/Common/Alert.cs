using System;
using UnityEngine;

public class Alert
{
	//MessageView view
	public static void show(string content, string okText, string cancelText, CallBack<object[]> ok = null, object[] okArgs = null, CallBack<object[]> cancel = null, object[] cancelArgs = null)
	{
		//      if (view == null)
		//          ;
		//view.show
        Debug.LogFormat("Alert.Show:{0}", content);
	}

	public static void show(string content, string okText, CallBack<object[]> ok = null, object[] okArgs = null)
    {
		Debug.LogFormat("Alert.Show:{0}", content);

	}

	public static void showContent(string content)
    {
		Debug.LogFormat("Alert.Show:{0}", content);

	}


}

