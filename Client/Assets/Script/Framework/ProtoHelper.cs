using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProtoHelper
{
    public static byte[] Serialize<T>(T t) {
        var stream = new MemoryStream();
        Serializer.Serialize<T>(stream, t);
        var bytes = stream.ToArray();
        stream.Close();
        stream.Dispose();
        return bytes;
    }


    public static T Deserialize<T>(byte[] bytes) {
        var stream = new MemoryStream(bytes);
        var t = Serializer.Deserialize<T>(stream);
        stream.Close();
        stream.Dispose();
        return t;
    }

    public static System.Object Deserialize(byte[] bytes, Type type) {
        var stream = new MemoryStream(bytes);
        var t = Serializer.Deserialize(type,stream);
        stream.Close();
        stream.Dispose();
        return t;
    }


    public static String SerializeToString<T>(T t) {
        return System.Text.Encoding.UTF8.GetString(Serialize(t));
    }

    public static T DeserializeFromString<T>(String s) {
        return Deserialize<T>(System.Text.Encoding.UTF8.GetBytes(s));
    }
}

