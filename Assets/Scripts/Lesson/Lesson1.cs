using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Lesson1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //数组初始化
        byte[] ipAddress = new byte[] { 118, 102, 111, 11 };
        IPAddress ip1 = new IPAddress(ipAddress);

        //long初始化,上面的十位转16位填入
        IPAddress ip2 = new IPAddress(0x76666F0B);

        //字符串转换
        IPAddress ip3 = IPAddress.Parse("118.102.111.11");


        //IP地址加端口，用来指定程序接口
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("118.102.111.11"),8080);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
