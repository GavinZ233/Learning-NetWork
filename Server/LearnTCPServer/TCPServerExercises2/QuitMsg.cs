using System.Collections;
using System.Collections.Generic;

public class QuitMsg : BaseMsg
{

    public override int GetBytesNum()
    {
        return 8;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {

        return 0;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteInt(bytes, GetID(), ref index);
        //写入消息体的长度
        WriteInt(bytes, 0, ref index);
        return bytes;
    }

    public override int GetID()
    {
        return 9999;
    }

}
