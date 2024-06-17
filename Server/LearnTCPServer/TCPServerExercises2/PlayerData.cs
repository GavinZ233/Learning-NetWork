using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PlayerData : BaseData
{
    public string name;
    public int atk;
    public int lev;


    public override int GetBytesNum()
    {
        return 12 + Encoding.UTF8.GetBytes(name).Length;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        name=ReadString(bytes,ref index);
        atk= ReadInt(bytes, ref index);
        lev= ReadInt(bytes, ref index);
        return index;
    }

    public override byte[] Writing()
    {
        byte[] bytes = new byte[GetBytesNum()];
        int index = 0;
        WriteString(bytes,name,ref index);
        WriteInt(bytes, atk, ref index);
        WriteInt(bytes, lev, ref index);
        return bytes;
    }

   
}
