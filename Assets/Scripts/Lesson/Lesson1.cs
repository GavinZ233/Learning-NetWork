using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Lesson1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //�����ʼ��
        byte[] ipAddress = new byte[] { 118, 102, 111, 11 };
        IPAddress ip1 = new IPAddress(ipAddress);

        //long��ʼ��,�����ʮλת16λ����
        IPAddress ip2 = new IPAddress(0x76666F0B);

        //�ַ���ת��
        IPAddress ip3 = IPAddress.Parse("118.102.111.11");


        //IP��ַ�Ӷ˿ڣ�����ָ������ӿ�
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("118.102.111.11"),8080);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
