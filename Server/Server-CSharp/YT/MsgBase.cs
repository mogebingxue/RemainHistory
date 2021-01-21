﻿using ProtoBuf;
using System;
using System.IO;
using System.Linq;

namespace YT
{
	public class MsgBase
	{
		public string protoName = "null";
	
	
	    //将protobuf对象序列化位byte数组
	    public static byte[] Encode(MsgBase msgBase) {
			return ProtoHelper.Serialize(msgBase);
		}
	    //解码
	    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count) {
			byte[] bodyBytes = new byte[count];
			Array.Copy(bytes, offset, bodyBytes, 0, count);
	        return (MsgBase)ProtoHelper.Deserialize(bodyBytes, Type.GetType(protoName));
	    }
	    //编码协议名（2字节长度+字符串）
	    public static byte[] EncodeName(MsgBase msgBase){
			//名字bytes和长度
			byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
	        short len = (short)nameBytes.Length;
			//申请bytes数值
			byte[] bytes = new byte[2+len];
			//组装2字节的长度信息
			bytes[0] = (byte)(len%256);
			bytes[1] = (byte)(len/256);
			//组装名字bytes
			Array.Copy(nameBytes, 0, bytes, 2, len);
	
			return bytes;
		}
	
		//解码协议名（2字节长度+字符串）
		public static string DecodeName(byte[] bytes, int offset, out int count){
			count = 0;
			//必须大于2字节
			if(offset + 2 > bytes.Length){
				return "";
			}
	        //读取长度
	        int len = ((bytes[offset+1] << 8 )| bytes[offset] );
			if(len <= 0){
				return "";
			}
			//长度必须足够
			if(offset + 2 + len > bytes.Length){
				return "";
			}
			//解析
			count = 2+len;
			string name = System.Text.Encoding.UTF8.GetString(bytes, offset+2, len);
			return name;
		}
	}
	
}

