using System;

//MD5信息
[Serializable]
public class MD5Message
{
    public string file;
    public string md5;
    public string fileLength;//文件长度
}

//MD5全部信息
[Serializable]
public class FileMD5
{
    public string length;
    public MD5Message[] versionInfo;
}

