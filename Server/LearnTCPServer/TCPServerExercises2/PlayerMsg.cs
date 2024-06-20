
public class PlayerMsg : BaseMsg
{
    public int playerID;

    public PlayerData playerData;

    public override int GetBytesNum()
    {
        return 4 +//消息长度
            4 +//ID
            4 +//playerID
            playerData.GetBytesNum();
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        playerID = ReadInt(bytes, ref index);
        playerData = ReadData<PlayerData>(bytes, ref index);
        return index;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteInt(bytes, GetID(), ref index);
        //写入消息体的长度
        WriteInt(bytes, bytesNum - 8, ref index);

        WriteInt(bytes, playerID, ref index);
        WriteData(bytes, playerData, ref index);
        return bytes;
    }

    public override int GetID()
    {
        return 1001;
    }


}
