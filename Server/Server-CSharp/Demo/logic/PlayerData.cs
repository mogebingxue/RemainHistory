
using ProtoBuf;

[ProtoContract]
public class PlayerData
{
    //头像
    [ProtoMember(1)]
    public int headPhoto = 0;
    //个人简介

    [ProtoMember(2)]
    public string palyerIntroduction = "new palyerIntroduction";
}