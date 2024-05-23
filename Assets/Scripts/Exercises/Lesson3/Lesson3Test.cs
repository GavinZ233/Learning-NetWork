using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
public class Lesson3Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerTest test = new PlayerTest();
        test.player = new Player() { ID = 3 };
        test.name = "∞¢À…¥Û";
        test.lev = 3;
        test.sex = true;

        byte[] bytes = test.Writing();

        PlayerTest test1 = new PlayerTest();
        int length =test1.Reading(bytes);
        Debug.Log(length + "  " + test1.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Player : BaseData
{
    public int ID;

    public override int GetBytesNum()
    {
        return 4;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        ID = ReadInt(bytes,ref beginIndex);
        return 4;
    }

    public override byte[] Writing()
    {
        return BitConverter.GetBytes(ID);
    }
}

public class PlayerTest : BaseData
{
    public Player player;
    public int lev;
    public string name;
    public bool sex;
    public override int GetBytesNum()
    {
        return sizeof(int) + player.GetBytesNum() + sizeof(int) + 
            Encoding.UTF8.GetBytes(name).Length + sizeof(bool);
    }

    public override int Reading(byte[] bytes,int beginIndex=0)
    {
        int index = beginIndex;
        player = ReadData<Player>(bytes,ref index);
        lev = ReadInt(bytes, ref index);
        name = ReadString(bytes, ref index);
        sex = ReadBool(bytes, ref index);

        return index-beginIndex;
    }

    public override byte[] Writing()
    {
        byte[] bytes = new byte[GetBytesNum()];
        int index = 0;

        WriteData(bytes,player,ref index);
        WriteInt(bytes, lev,ref index);
        WriteString(bytes, name, ref index);
        WriteBool(bytes, sex, ref index);
        return bytes;
    }
}
