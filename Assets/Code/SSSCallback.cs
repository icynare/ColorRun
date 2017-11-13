using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////SDK回调脚本
////多个参数按照顺序以文本："aaa|bbb|ccc拆分


public class SSSCallback : MonoBehaviour
{

    ///微信登录成功回调
    public void OnWxAuthReqSucc(string code)
    {
        Alert.showContent("微信授权成功");
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
        //ProtoRequest.instance.ReqAuthLogin("", "", 1, code,GameData.GetDevice(), SSSPlugin.getDeviceId());
#endif
    }

    ///微信支付成功回调
    public void OnWxPaySucc(string str)
    {
        Alert.showContent("支付成功");
    }

    public void OnIAPPaySucc(string base64Receipt){
        //todo:发送回执协议-ProtoRequest.instance.ReqBuy(0,3,base64Receipt);
    }

    //qq登录成功
    public void OnQQAuthReqSucc(string msg)
    {
        Alert.showContent("QQ授权成功");
#if UNITY_ANDROID
        string[] tmp = msg.Split('|');
#elif UNITY_IOS
		string[] tmp = msg.Split('_');
#endif

#if UNITY_ANDROID || UNITY_IOS
        string openId = tmp[0];
		string accessToken = tmp[1];

        //ProtoRequest.instance.ReqAuthLogin(openId, accessToken, 2, "",GameData.GetDevice(),SSSPlugin.getDeviceId());
#endif

    }
    /// <summary>
    /// share　empty callback
    /// </summary>
    /// <param name="str"></param>
    public void OnS(string str)
    {
        Alert.showContent("邀请成功");
    }

    public void OnRoomDataShare(string str)
    {
        //XEventSystem.Emit(XEvent.onRoomDataShare);
		Alert.showContent("发送成功");

	}

    public void OnUserShare(string str)
    {
        Alert.showContent("发送成功");
    }

    public void OnError(string str){
		Alert.showContent(str);

	}
}
