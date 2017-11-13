using UnityEngine;
using System.Collections;
using System.IO;
using Google.Protobuf;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Text;

public class ProtoBufTool
{
    public const int HEAD_SIZE = 6; //code:2B+length:4B
	public const int CODE_SIZE = 2;
	public const int CODE_BEGIN = 0;
	public const int LENGTH_SIZE = 4;
	public const int LENGTH_BEGIN = 2;
    public const int MAX_PACKET_SIZE = 4 * 1024;

    //delete
    //public static byte[] ToByteArray<T>(uint _type, T _instance, ref int _length)
    //{
    //    using (var stream = new MemoryStream())
    //    {
    //        ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, _instance, ProtoBuf.PrefixStyle.None);
    //        _length = (int)stream.Length;
    //        if (_length >= MAX_PACKET_SIZE - HEAD_SIZE)
    //        {
    //            Assert.IsTrue(false, "net message stream serialize error!".Color());
    //            return null;
    //        }

    //        var buff = new byte[_length];
    //        var result = new byte[_length + HEAD_SIZE];

    //        stream.Position = 0;
    //        stream.Read(buff, 0, _length);
    //        result.WriteBytes((_length + HEAD_SIZE/2).ToBytes().ReverseHead(), 0);
    //        result.WriteBytes(_type.ToBytes().ReverseHead(), 4);
    //        Array.Copy(buff, 0, result, HEAD_SIZE, _length);

    //        return result;
    //    }
    //}

    //public static T Parse<T>(byte[] _data)
    //{
    //    T result;
    //    using (var stream = new MemoryStream(_data, HEAD_SIZE, _data.Length - HEAD_SIZE))
    //    {
    //        result = ProtoBuf.Serializer.Deserialize<T>(stream);
    //        stream.Close();
    //    }
    //    return result;
    //}

    public static byte[] ToByteArray(ushort msgCode, IMessage instance, ref int bodyLength)
	{

        using (MemoryStream stream = new MemoryStream())
		{
            instance.WriteTo(stream);
            //_instance.WriteTo(stream);
            //pb.
            //ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, _instance, ProtoBuf.PrefixStyle.None);
            bodyLength = (int)stream.Length;

			if (bodyLength >= MAX_PACKET_SIZE - HEAD_SIZE)
			{
				Assert.IsTrue(false, "net message stream serialize to long!".Color());
				return null;
			}

			var buff = new byte[bodyLength];
			var result = new byte[bodyLength + HEAD_SIZE];

			stream.Position = 0;
			stream.Read(buff, 0, (int)bodyLength);
			result.WriteBytes(msgCode.ToBytes().ReverseHead(), 0);
			result.WriteBytes(bodyLength.ToBytes().ReverseHead(), 2);
            Array.Copy(buff, 0, result, HEAD_SIZE, bodyLength);

			return result;
		}
	}

	//public static T Parse<T>(byte[] _data)
	//{
	//	T result;
	//	using (var stream = new MemoryStream(_data, HEAD_SIZE, _data.Length - HEAD_SIZE))
	//	{
	//		result = ProtoBuf.Serializer.Deserialize<T>(stream);
	//		stream.Close();
	//	}
	//	return result;
	//}
    //
    public static IMessage Parse(byte[] _data,Type t,int index,int length)
	{
        IMessage result;
        //Debug.Log($"index:{index} innerlength:{length}");
		Type[] params_type = new Type[1];
		params_type[0] = _data.GetType();
		//设置Show_Str方法中的参数值；如有多个参数可以追加多个   
		object[] params_obj = new object[1];
        byte[] cacheBytes = new byte[length];
        //Array.Copy(cacheBytes, bytes, cacheBytes.Length);
        Array.Copy(_data, index, cacheBytes,0 , length);
        //Debug.Log(cacheBytes);
		params_obj[0] = cacheBytes;
		//var sb = new StringBuilder();
		//for (int i = 0; i < _data.Length; i++)
		//{
		//	sb.AppendLine(_data[i].ToString());
		//}
		//Debug.Log($"receive message -> \n{sb}");
		var a = t.GetProperty("Parser").GetValue(null, null);
		var b = a.GetType().GetMethod("ParseFrom", params_type).Invoke(a, params_obj);

		
		//result = ProtoBuf.Serializer.Deserialize(stream,t);
        result = b as IMessage;
        return result;
	}



}
