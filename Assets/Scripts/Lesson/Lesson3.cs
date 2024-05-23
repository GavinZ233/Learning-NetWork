using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Lesson3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestInfo testInfo = new TestInfo();
        testInfo.lev = 33;
        testInfo.name = "¥ÛÀ’¥Ú";
        testInfo.atk = 32;
        testInfo.sex = true;
        byte[] bs =testInfo.GetBytes();
        Debug.Log(bs.Length);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class TestInfo{
    public int lev;
    public string name;
    public short atk;
    public bool sex;

    public byte[] GetBytes()
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(name);
        int length = sizeof(int)+sizeof(int)+ strBytes.Length+sizeof(short)+sizeof(bool);
        byte[] bytes = new byte[length];
        int index = 0;
        BitConverter.GetBytes(lev).CopyTo(bytes,index);
        index += sizeof(int);
        BitConverter.GetBytes(strBytes.Length).CopyTo(bytes, index);
        index += sizeof(int);
        strBytes.CopyTo(bytes, index);
        index += strBytes.Length;
        BitConverter.GetBytes(atk).CopyTo(bytes, index);
        index += sizeof(short);
        BitConverter.GetBytes(sex).CopyTo(bytes, index);


        return strBytes;
    }

}