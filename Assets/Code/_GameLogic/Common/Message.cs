using System;
using UnityEngine;

public class Message
{
    //MessageView view
	public static void show(string content)
	{
        //      if (view == null)
        //          ;
        //view.show
        Debug.LogFormat("Message.Show:{0}", content);

	}
}

