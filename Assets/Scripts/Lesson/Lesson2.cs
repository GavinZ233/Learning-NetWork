using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IPHostEntry host = new IPHostEntry();

        print(Dns.GetHostName());

        IPHostEntry entry = Dns.GetHostEntry("www.baidu.com");

        for (int i = 0; i < entry.AddressList.Length; i++)
        {
            print("IP��ַ��"+entry.AddressList[i]);
        }
        for (int i = 0; i < entry.Aliases.Length; i++)
        {
            print("������" + entry.Aliases[i]);

        }
        print(entry.HostName);

         GetHostEntry();

    }

    
    private async void GetHostEntry()
    {
        Debug.Log("�첽");


        Task<IPHostEntry> task =Dns.GetHostEntryAsync("www.baidu.com");

        await task;

        for (int i = 0; i < task.Result.AddressList.Length; i++)
        {
            print("IP��ַ��" + task.Result.AddressList[i]);
        }
        for (int i = 0; i < task.Result.Aliases.Length; i++)
        {
            print("������" + task.Result.Aliases[i]);

        }
        print(task.Result.HostName);

    }



}
