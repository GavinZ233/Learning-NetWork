using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
public abstract class BaseData 
{
    #region 序列化

    public abstract int GetBytesNum();

    public abstract byte[] Writing();

    protected void WriteShort(byte[] bytes,short value,ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes,index);
        index += sizeof(short);
    }

    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(bool);
    }
    protected void WriteInt(byte[] bytes, int value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(int);
    }
    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(float);
    }
    protected void WriteLong(byte[] bytes, long value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(long);
    }
    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        byte[] strBs = Encoding.UTF8.GetBytes(value);
        BitConverter.GetBytes(strBs.Length).CopyTo(bytes, index);
        index += sizeof(int);
        strBs.CopyTo(bytes, index);
        index += strBs.Length;

    }
    protected void WriteData(byte[] bytes,BaseData data,ref int index)
    {
        data.Writing().CopyTo(bytes, index);
        index += data.GetBytesNum();
    }

    #endregion

    #region 反序列化

    public abstract int Reading(byte[] bytes,int beginIndex=0);

    protected int ReadInt(byte[] bytes,ref int index)
    {
        int value = BitConverter.ToInt32(bytes,index);
        index += sizeof(int);
        return value;
    }
    protected bool ReadBool(byte[] bytes, ref int index)
    {
        bool value = BitConverter.ToBoolean(bytes, index);
        index += sizeof(bool);
        return value;
    }
    protected float ReadFloat(byte[] bytes, ref int index)
    {
        float value = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return value;
    }

    protected string ReadString(byte[] bytes, ref int index)
    {
        int length = ReadInt(bytes,ref index);
        string value = Encoding.UTF8.GetString(bytes,index,length);
        index += length;
        return value;
    }

    protected T ReadData<T>(byte[] bytes,ref int index) where T:BaseData,new()
    {
        T value = new T();

        index += value.Reading(bytes,index);

        return value;
    }

    #endregion

}
