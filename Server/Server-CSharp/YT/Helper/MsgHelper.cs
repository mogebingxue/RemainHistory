using ENet;
using Google.Protobuf;
using System;
using System.IO;
using System.Linq;

namespace YT
{
	class MsgHelper
	{
	
		/// <summary>
		/// 编码
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static byte[] Encode(IMessage msg) {
			//数据编码
			byte[] nameBytes = EncodeName(msg);
			byte[] bodyBytes = EncodeBody(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //组装长度
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            //组装名字
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            //组装消息体
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            return sendBytes;
        }

		/// <summary>
		/// 将protobuf对象序列化位byte数组
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private static byte[] EncodeBody(IMessage msg) {
			return msg.ToByteArray();
			
		}

		/// <summary>
		/// 解码
		/// </summary>
		/// <returns></returns>
		public static Request Decode(ByteArray readBuff,Peer peer) {
            //消息长度
            if (readBuff.length <= 2) {
                return null;
            }
            //消息体长度
            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.length < bodyLength) {
                return null;
            }
            readBuff.readIdx += 2;
            //解析协议名
            int nameCount = 0;
            string protoName = MsgHelper.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            if (protoName == "") {
                Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
                //Close(user);
                return null;
            }
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            if (bodyCount < 0) {
                Console.WriteLine("OnReceiveData fail, bodyCount <0 ");
                //Close(user);
                return null;
            }
			byte[] msg = DecodeBody( readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            Request request = new Request();
            request.Peer = peer;
            request.Name = protoName;
            request.Msg = msg;
            return request;
        }
	    
		//解码协议体
	    private static byte[] DecodeBody( byte[] bytes, int offset, int count) {
			byte[] bodyBytes = new byte[count];
			Array.Copy(bytes, offset, bodyBytes, 0, count);
	        return bodyBytes;
	    }
	    
		//编码协议名（2字节长度+字符串）
	    private static byte[] EncodeName(IMessage msg){
			//名字bytes和长度
			byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.GetType().Name);
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
		private static string DecodeName(byte[] bytes, int offset, out int count){
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

