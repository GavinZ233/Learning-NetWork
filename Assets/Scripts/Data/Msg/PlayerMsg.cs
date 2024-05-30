using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMsg : BaseMsg
{
    public int playerID;

    public PlayerData playerData;

    public override int GetBytesNum()
    {
        return 8 + playerData.GetBytesNum();
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        playerID = ReadInt(bytes, ref index);
        playerData =ReadData<PlayerData>(bytes, ref index);
        return index;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes,GetID(),ref index);
        WriteInt(bytes, playerID, ref index);
        WriteData(bytes, playerData, ref index);
        return bytes;
    }

    public override int GetID()
    {
        return 1001;
    }


}
