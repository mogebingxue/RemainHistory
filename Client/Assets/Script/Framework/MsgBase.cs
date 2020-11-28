using ProtoBuf;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class MsgBase {
    public string protoName = "null";
    //将protobuf对象序列化位byte数组
    public static byte[] Encode(MsgBase msgBase) {
        return ProtoHelper.Serialize(msgBase);
    }
    //解码
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count) {
        byte[] bodyBytes = new byte[count];
        Array.Copy(bytes, offset, bodyBytes, 0, count);
        if (protoName == "MsgLogin") {
            return ProtoHelper.Deserialize<MsgLogin>(bodyBytes);
        }
        if (protoName == "MsgRegister") {
            return ProtoHelper.Deserialize<MsgRegister>(bodyBytes);
        }
        if (protoName == "MsgKick") {
            return ProtoHelper.Deserialize<MsgKick>(bodyBytes);
        }
        if (protoName == "MsgGetPlayerIntroduction") {
            return ProtoHelper.Deserialize<MsgGetPlayerIntroduction>(bodyBytes);
        }
        if (protoName == "MsgSavePlayerIntroduction") {
            return ProtoHelper.Deserialize<MsgSavePlayerIntroduction>(bodyBytes);
        }
        if (protoName == "MsgGetHeadPhoto") {
            return ProtoHelper.Deserialize<MsgGetHeadPhoto>(bodyBytes);
        }
        if (protoName == "MsgSaveHeadPhoto") {
            return ProtoHelper.Deserialize<MsgSaveHeadPhoto>(bodyBytes);
        }
        if (protoName == "MsgSendMessageToWord") {
            return ProtoHelper.Deserialize<MsgSendMessageToWord>(bodyBytes);
        }
        if (protoName == "MsgSendMessageToFriend") {
            return ProtoHelper.Deserialize<MsgSendMessageToFriend>(bodyBytes);
        }
        if (protoName == "MsgGetFriendList") {
            return ProtoHelper.Deserialize<MsgGetFriendList>(bodyBytes);
        }
        if (protoName == "MsgGetFriendList") {
            return ProtoHelper.Deserialize<MsgGetFriendList>(bodyBytes);
        }
        if (protoName == "MsgDeleteFriend") {
            return ProtoHelper.Deserialize<MsgDeleteFriend>(bodyBytes);
        }
        if (protoName == "MsgAddFriend") {
            return ProtoHelper.Deserialize<MsgAddFriend>(bodyBytes);
        }
        if (protoName == "MsgAcceptAddFriend") {
            return ProtoHelper.Deserialize<MsgAcceptAddFriend>(bodyBytes);
        }
        if (protoName == "MsgAcceptAddFriend") {
            return ProtoHelper.Deserialize<MsgAcceptAddFriend>(bodyBytes);
        }
        if (protoName == "MsgPing") {
            return ProtoHelper.Deserialize<MsgPing>(bodyBytes);
        }
        if (protoName == "MsgPong") {
            return ProtoHelper.Deserialize<MsgPong>(bodyBytes);
        }
        else {
            return null;
        }
    }

    //编码协议名（2字节长度+字符串）
    public static byte[] EncodeName(MsgBase msgBase) {
        //名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        //申请bytes数值
        byte[] bytes = new byte[2 + len];
        //组装2字节的长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }

    //解码协议名（2字节长度+字符串）
    public static string DecodeName(byte[] bytes, int offset, out int count) {
        count = 0;
        //必须大于2字节
        if (offset + 2 > bytes.Length) {
            return "";
        }
        //读取长度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        if (len <= 0) {
            return "";
        }
        //长度必须足够
        if (offset + 2 + len > bytes.Length) {
            return "";
        }
        //解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }
}


